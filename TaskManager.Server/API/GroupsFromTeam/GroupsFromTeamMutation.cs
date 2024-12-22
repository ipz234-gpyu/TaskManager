using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.GroupsFromTeam.Types;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.GroupsFromTeam
{
    public class GroupsFromTeamMutation : ObjectGraphType
    {
        public GroupsFromTeamMutation(IGroupsFromTeamRepository groupsFromTeamRepository)
        {
            this.Authorize();

            Field<GroupFromTeamResponsType>("createGroupFromTeam")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "priority" }
                    ))
                .ResolveAsync(async context => {
                    var teamId = context.GetArgument<Guid>("teamId");
                    var nameGroup = context.GetArgument<string>("name");
                    var priority = context.GetArgument<int>("priority");

                    var groupsFromTeam = new GroupFromTeam()
                    {
                        TeamId = teamId,
                        Name = nameGroup,
                        Priority = priority
                    };

                    return await groupsFromTeamRepository.CreateAsync(groupsFromTeam);
                });

            Field<GroupFromTeamResponsType>("updateGroupFromTeam")
               .Arguments(new QueryArguments(
                   new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupFromTeamId" },
                   new QueryArgument<StringGraphType> { Name = "name" },
                   new QueryArgument<IntGraphType> { Name = "priority" }
                   ))
               .ResolveAsync(async context => {
                   var groupFromTeamId = context.GetArgument<Guid>("groupFromTeamId");
                   var nameGroup = context.GetArgument<string>("name");
                   var priority = context.GetArgument<int?>("priority", null);

                   var groupsFromUser = await groupsFromTeamRepository.GetByIdAsync(groupFromTeamId);

                   groupsFromUser.Name = nameGroup ?? groupsFromUser.Name;
                   groupsFromUser.Priority = priority ?? groupsFromUser.Priority;

                   return await groupsFromTeamRepository.UpdateAsync(groupsFromUser);
               });

            Field<BooleanGraphType>("deleteGroupFromUser")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupFromTeamId" }
            ))
            .ResolveAsync(async context => {
                    var groupFromTeamId = context.GetArgument<Guid>("groupFromTeamId");

                    await groupsFromTeamRepository.DeleteAsync(groupFromTeamId);
                    return true;
                });
        }
    }
}
