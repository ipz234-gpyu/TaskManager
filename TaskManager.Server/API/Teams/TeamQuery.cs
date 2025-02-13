﻿using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Teams.Types;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Teams;

public class TeamQuery : ObjectGraphType
{
    public TeamQuery(ITeamRepository teamRepository, IUserRepository userRepository)
    {
        this.Authorize();

        Field<ListGraphType<TeamResponsType>>("getAllGroupFromUser")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" }
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
            return await teamRepository.GetAllByUserId(user.UserId);
        });

        Field<ListGraphType<TaskAssignmentResponsType>>("getAllTaskAssignments")
         .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" }
                ))
        .ResolveAsync(async context =>
        {
            var teamId = context.GetArgument<Guid>("teamId");
            var team = await teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                return null;
            }

            return await teamRepository.GetAllTaskAssignments(teamId);
        });
    }
}
