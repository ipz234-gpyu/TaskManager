using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.GroupsFromTeam.Types;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.GroupsFromTeam;

public class GroupsFromTeamQuery : ObjectGraphType
{
    public GroupsFromTeamQuery(IGroupsFromTeamRepository groupsFromTeamRepository)
    {
        this.Authorize();

        Field<ListGraphType<GroupFromTeamResponsType>>("getAllGroupFromTeam")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" }
                ))
            .ResolveAsync(async context =>
            {
                var teamId = context.GetArgument<Guid>("teamId");
                return await groupsFromTeamRepository.GetAllForTeamAsync(teamId);
            });

        Field<GroupFromTeamResponsType>("getGroupFromTeamById")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupFromTeamId" }
                ))
            .ResolveAsync(async context =>
            {
                var groupFromUserId = context.GetArgument<Guid>("groupFromTeamId");
                return await groupsFromTeamRepository.GetByIdAsync(groupFromUserId);
            });
    }
}
