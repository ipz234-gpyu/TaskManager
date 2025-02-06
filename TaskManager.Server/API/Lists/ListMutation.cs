using GraphQL;
using GraphQL.Types;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.Interfaces;
using TaskManager.Server.API.Lists.Types;
using TaskManager.Server.Domain.Errors;

namespace TaskManager.Server.API.Lists;

public class ListMutation : ObjectGraphType
{
    public ListMutation(IListRepository listRepository,
        IGroupsFromTeamRepository fromTeamRepository,
        IGroupsFromUserRepository fromUserRepository)
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
               var group = await fromUserRepository.GetByIdAsync(groupId);
               if (group == null)
               {
                   context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                   return null;
               }
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
               var group = await fromTeamRepository.GetByIdAsync(groupId);
               if (group == null)
               {
                   context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                   return null;
               }
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
                if (list == null)
                {
                    context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                    return null;
                }

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

        Field<BooleanGraphType>("removeListFromUserGroup")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
               ))
           .ResolveAsync(async context => {
               var groupId = context.GetArgument<Guid>("groupId");
               var group = await fromUserRepository.GetByIdAsync(groupId);
               if (group == null)
               {
                   context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                   return false;
               }

               var listId = context.GetArgument<Guid>("listId");
               var list = await listRepository.GetByIdAsync(listId);
               if (list == null)
               {
                   context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                   return false;
               }
               
               await listRepository.DeleteConnectionForUser(group.GroupsFromUserId, list.ListId);

               return true;
           });

        Field<BooleanGraphType>("addListForUserGroup")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
               ))
           .ResolveAsync(async context => {
               var groupId = context.GetArgument<Guid>("groupId");
               var group = await fromUserRepository.GetByIdAsync(groupId);
               if (group == null)
               {
                   context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                   return false;
               }

               var listId = context.GetArgument<Guid>("listId");
               var list = await listRepository.GetByIdAsync(listId);
               if (list == null)
               {
                   context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                   return false;
               }

               await listRepository.CreateConnectionForUser(group.GroupsFromUserId, list.ListId);

               return true;
           });

        Field<BooleanGraphType>("removeListFromTeamGroup")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
               ))
           .ResolveAsync(async context => {
               var groupId = context.GetArgument<Guid>("groupId");
               var group = await fromTeamRepository.GetByIdAsync(groupId);
               if (group == null)
               {
                   context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                   return false;
               }

               var listId = context.GetArgument<Guid>("listId");
               var list = await listRepository.GetByIdAsync(listId);
               if (list == null)
               {
                   context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                   return false;
               }

               await listRepository.DeleteConnectionForTeam(group.GroupsFromTeamId, list.ListId);

               return true;
           });

        Field<BooleanGraphType>("addListForTeamGroup")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
               ))
           .ResolveAsync(async context => {
               var groupId = context.GetArgument<Guid>("groupId");
               var group = await fromTeamRepository.GetByIdAsync(groupId);
               if (group == null)
               {
                   context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                   return false;
               }

               var listId = context.GetArgument<Guid>("listId");
               var list = await listRepository.GetByIdAsync(listId);
               if (list == null)
               {
                   context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                   return false;
               }

               await listRepository.CreateConnectionForTeam(group.GroupsFromTeamId, list.ListId);

               return true;
           });
    }
}
