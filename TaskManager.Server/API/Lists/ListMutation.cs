using GraphQL;
using GraphQL.Types;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.Interfaces;
using TaskManager.Server.API.Lists.Types;
using TaskManager.Server.Domain.Errors;

namespace TaskManager.Server.API.Lists
{
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
                   if(group == null)
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

            Field<BooleanGraphType>("MoveUserListTo")
               .Arguments(new QueryArguments(
                   new QueryArgument<GuidGraphType> { Name = "removedGroupId" },
                   new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "addGroupId" },
                   new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
                   ))
               .ResolveAsync(async context => {
                   var addGroupId = context.GetArgument<Guid>("addGroupId");
                   var addGroup = await fromUserRepository.GetByIdAsync(addGroupId);
                   if (addGroup == null)
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

                   await listRepository.CreateConnectionForUser(addGroup.GroupsFromUserId, list.ListId);

                   var removedGroupId = context.GetArgument<Guid?>("removedGroupId");
                   if (removedGroupId.HasValue)
                   {
                       var removedGroup = await fromUserRepository.GetByIdAsync(removedGroupId.Value);
                       if (removedGroup == null || addGroup.GroupsFromUserId == removedGroup.GroupsFromUserId)
                       {
                           context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                           return false;
                       }
                       else await listRepository.DeleteConnectionForUser(removedGroup.GroupsFromUserId, list.ListId);
                   }

                   return true;
               });

            Field<BooleanGraphType>("MoveTeamListTo")
               .Arguments(new QueryArguments(
                   new QueryArgument<GuidGraphType> { Name = "removedGroupId" },
                   new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "addGroupId" },
                   new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
                   ))
               .ResolveAsync(async context => {
                   var addGroupId = context.GetArgument<Guid>("addGroupId");
                   var addGroup = await fromTeamRepository.GetByIdAsync(addGroupId);
                   if (addGroup == null)
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

                   await listRepository.CreateConnectionForTeam(addGroup.GroupsFromTeamId, list.ListId);

                   var removedGroupId = context.GetArgument<Guid?>("removedGroupId");
                   if (removedGroupId.HasValue)
                   {
                       var removedGroup = await fromTeamRepository.GetByIdAsync(removedGroupId.Value);
                       if (removedGroup == null || addGroup.GroupsFromTeamId == removedGroup.GroupsFromTeamId)
                       {
                           context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                           return false;
                       }
                       else await listRepository.DeleteConnectionForTeam(removedGroup.GroupsFromTeamId, list.ListId);
                   }

                   return true;
               });
        }
    }
}
