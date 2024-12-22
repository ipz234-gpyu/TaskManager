using GraphQL;
using GraphQL.Types;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Services;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Teams
{
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
            Field<BooleanGraphType>("inviteToTeam")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "UserEmail" },
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "TeamId" }
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

                    var teamId = context.GetArgument<Guid>("TeamId");
                    var team = await teamRepository.GetByIdAsync(teamId);
                    if (team == null)
                    {
                        context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
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
                    await emailSender.SendInviteToTeamEmailAsync(token, user, team);

                    return true;
                });
        }
    }
}
