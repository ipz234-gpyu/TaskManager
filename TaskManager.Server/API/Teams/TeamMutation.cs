using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using TaskManager.Server.API.Teams.Types;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Services;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Teams;

public class TeamMutation : ObjectGraphType
{
    public TeamMutation(
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        ITeamInvitationRepository tokenRepository,
        IUserTeamRepository userTeamRepository,
        IJwtTokenUtils jwtTokenUtils
        )
    {
        this.Authorize();

        Field<TeamResponsType>("createTeam")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "teamName" }
            ))
            .ResolveAsync(async context =>
            {
                var userId = context.GetArgument<Guid>("userId");
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                    return false;
                }

                var teamName = context.GetArgument<string>("teamName");
                var team = new Team()
                {
                    NameTeam = teamName,
                    CreatedBy = user.UserId
                };
                
                return await teamRepository.CreateAsync(team); ;
            });

        Field<BooleanGraphType>("inviteToTeam")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userSenderInviteId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userEmail" },
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" }
            ))
            .ResolveAsync(async context =>
            {
                var userEmail = context.GetArgument<string>("userEmail");
                if (!DataValidator.IsEmailValid(userEmail))
                {
                    context.Errors.Add(ErrorCode.INVALID_EMAIL_FORMAT);
                    return false;
                }

                var user = await userRepository.GetUserByEmailAsync(userEmail);
                if (user == null)
                {
                    context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                    return false;
                }

                var teamId = context.GetArgument<Guid>("teamId");
                var team = await teamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                    return false;
                }

                var userSenderInviteId = context.GetArgument<Guid>("userSenderInviteId");
                var userSenderInvite = await userRepository.GetByIdAsync(userSenderInviteId);
                if (team.CreatedBy != userSenderInvite.UserId || userSenderInvite == null)
                {
                    context.Errors.Add(ErrorCode.ACCESS_RIGHTS_ERROR);
                    return false;
                }

                var userTeam = await userTeamRepository.GetByUserIdAndTeamId(team.TeamId, user.UserId);
                if (userTeam != null)
                {
                    context.Errors.Add(ErrorCode.USER_ALREADY_IN_THE_TEAM);
                    return false;
                }

                var userInvite = await tokenRepository.GetByUserIdAndTeamId(team.TeamId, user.UserId);
                if (userInvite != null)
                {
                    context.Errors.Add(ErrorCode.USER_HAS_ALREADY_BEEN_INVITED);
                    return false;
                }

                var token = await jwtTokenUtils.GenerateInviteToken(team.TeamId, user.UserId);
                var emailSender = context.RequestServices.GetRequiredService<IEmailSender>();
                await emailSender.InviteToTeamEmailSendAsync(token, user, team);

                return true;
            });

        Field<TeamResponsType>("acceptInviteToTeam")
         .Arguments(new QueryArguments(
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
            new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "token" }
        ))
         .ResolveAsync(async context =>
         {
             try
             {
                 var token = context.GetArgument<string>("token");

                 var principal = jwtTokenUtils.ValidateToken(token);

                 var tokenHash = principal.FindFirst("hash")?.Value;

                 var tokenIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception();

                 if (!Guid.TryParse(tokenIdClaim, out var tokenId))
                     throw new Exception("Invalid TokenId format.");

                 var tokenInvite = await tokenRepository.GetByIdAsync(tokenId) ?? throw new Exception();
                 var user = await userRepository.GetByIdAsync(tokenInvite.UserId) ?? throw new Exception();
                 var team = await teamRepository.GetByIdAsync(tokenInvite.TeamId) ?? throw new Exception();

                 if (user.UserId != tokenInvite.UserId || tokenInvite.TokenHash != tokenHash)
                     throw new Exception();

                 await userTeamRepository.CreateAsync(new() { UserId = user.UserId, TeamId = team.TeamId });
                 await tokenRepository.DeleteAsync(tokenInvite.TeamInvitationId);

                 return team;
             }
             catch
             {
                 context.Errors.Add(ErrorCode.LINK_NOT_FOUND);
                 return null;
             }
         });

        Field<TeamResponsType>("updateTeam")
         .Arguments(new QueryArguments(
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" },
            new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "newName" }
        ))
        .ResolveAsync(async context =>
        {
            var userId = context.GetArgument<Guid>("userId");
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                return null;
            }

            var teamId = context.GetArgument<Guid>("teamId");
            var team = await teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                return null;
            }

            if (user.UserId != team.CreatedBy)
            {
                context.Errors.Add(ErrorCode.ACCESS_RIGHTS_ERROR);
                return null;
            }
            var newName = context.GetArgument<string>("newName");
            team.NameTeam = newName;

            return await teamRepository.UpdateAsync(team);
        });

        Field<BooleanGraphType>("deleteTeam")
         .Arguments(new QueryArguments(
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" }
        ))
        .ResolveAsync(async context =>
        {
            var userId = context.GetArgument<Guid>("userId");
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                return false;
            }

            var teamId = context.GetArgument<Guid>("teamId");
            var team = await teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                return false;
            }

            if (user.UserId != team.CreatedBy)
            {
                context.Errors.Add(ErrorCode.ACCESS_RIGHTS_ERROR);
                return false;
            }

            await teamRepository.DeleteAsync(team.TeamId);
            return true;
        });

        Field<BooleanGraphType>("kickUserOfTheTeam")
         .Arguments(new QueryArguments(
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "kickUserId" },
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" }
        ))
        .ResolveAsync(async context =>
        {
            var userId = context.GetArgument<Guid>("userId");
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                return false;
            }

            var teamId = context.GetArgument<Guid>("teamId");
            var team = await teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                return false;
            }

            if (user.UserId != team.CreatedBy)
            {
                context.Errors.Add(ErrorCode.ACCESS_RIGHTS_ERROR);
                return false;
            }

            var kickUserId = context.GetArgument<Guid>("kickUserId");
            var kickUser = await userRepository.GetByIdAsync(kickUserId);
            if (kickUser == null)
            {
                context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                return false;
            }

            await userTeamRepository.DeleteByUserIdAndTeamId(team.TeamId, kickUser.UserId);
            return true;
        });


        Field<BooleanGraphType>("leaveTeam")
         .Arguments(new QueryArguments(
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
            new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" }
        ))
        .ResolveAsync(async context =>
        {
            var userId = context.GetArgument<Guid>("userId");
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                return false;
            }

            var teamId = context.GetArgument<Guid>("teamId");
            var team = await teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                return false;
            }

            await userTeamRepository.DeleteByUserIdAndTeamId(team.TeamId, user.UserId);
            return true;
        });
    }
}
