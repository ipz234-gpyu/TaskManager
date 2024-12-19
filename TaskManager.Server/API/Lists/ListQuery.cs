using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Lists.Types;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Lists
{
    public class ListQuery : ObjectGraphType
    {
        public ListQuery(IListRepository listRepository)
        {
            this.Authorize();

            Field<ListGraphType<ListResponsType>>("getAllListsGroupFromUser")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" }
                    ))
                .ResolveAsync(async context =>
                {
                    var groupId = context.GetArgument<Guid>("groupId");
                    return await listRepository.GetAllForGroupUserAsync(groupId);
                });

            Field<ListGraphType<ListResponsType>>("getAllListsGroupFromTeam")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" }
                    ))
                .ResolveAsync(async context =>
                {
                    var groupId = context.GetArgument<Guid>("groupId");
                    return await listRepository.GetAllForGroupTeamAsync(groupId);
                });
        }
    }
}
