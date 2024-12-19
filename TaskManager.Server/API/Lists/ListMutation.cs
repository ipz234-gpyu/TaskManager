using GraphQL;
using GraphQL.Types;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.Interfaces;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.API.Lists.Types;

namespace TaskManager.Server.API.Lists
{
    public class ListMutation : ObjectGraphType
    {
        public ListMutation(IListRepository listRepository)
        {
            this.Authorize();

            Field<ListResponsType>("createListGroupFromUser")
               .Arguments(new QueryArguments(
                   new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" },
                   new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                   new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "priority" }
                   ))
               .ResolveAsync(async context => {
                   var groupId = context.GetArgument<Guid>("groupId");
                   var nameGroup = context.GetArgument<string>("name");
                   var priority = context.GetArgument<int>("priority");

                   var groupsFromUser = new List()
                   {
                       Name = nameGroup,
                       Priority = priority
                   };

                   return await listRepository.CreateForUserAsync(groupId, groupsFromUser);
               });

            Field<ListResponsType>("createListGroupFromTeam")
               .Arguments(new QueryArguments(
                   new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" },
                   new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                   new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "priority" }
                   ))
               .ResolveAsync(async context => {
                   var groupId = context.GetArgument<Guid>("groupId");
                   var nameGroup = context.GetArgument<string>("name");
                   var priority = context.GetArgument<int>("priority");

                   var groupsFromUser = new List()
                   {
                       Name = nameGroup,
                       Priority = priority
                   };

                   return await listRepository.CreateForTeamAsync(groupId, groupsFromUser);
               });

            Field<ListResponsType>("updateList")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "priority" }
                    ))
                .ResolveAsync(async context => {
                    var listId = context.GetArgument<Guid>("listId");
                    var name = context.GetArgument<string>("name");
                    var priority = context.GetArgument<int?>("priority", null);

                    var list = await listRepository.GetByIdAsync(listId);

                    list.Name = name ?? list.Name;
                    list.Priority = priority ?? list.Priority;

                    return await listRepository.UpdateAsync(list);
                });

            Field<BooleanGraphType>("deleteList")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
                    ))
                .ResolveAsync(async context => {
                    var listId = context.GetArgument<Guid>("listId");
                    await listRepository.DeleteAsync(listId);
                    return true;
                });
        }
    }
}
