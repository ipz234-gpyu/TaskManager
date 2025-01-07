using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.GroupsFromUser.Types;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.GroupsFromUser;

public class GroupsFromUserMutation : ObjectGraphType
{
    public GroupsFromUserMutation(IGroupsFromUserRepository groupsFromUserRepository)
    {
        this.Authorize();

        Field<GroupFromUserResponsType>("createGroupFromUser")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "priority" }
                ))
            .ResolveAsync(async context => {
                var userId = context.GetArgument<Guid>("userId");
                var nameGroup = context.GetArgument<string>("name");
                var priority = context.GetArgument<int>("priority");

                var groupsFromUser = new GroupFromUser()
                {
                    UserId = userId,
                    Name = nameGroup,
                    Priority = priority
                };

                return await groupsFromUserRepository.CreateAsync(groupsFromUser);
            });

        Field<GroupFromUserResponsType>("updateGroupFromUser")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupFromUserId" },
               new QueryArgument<StringGraphType> { Name = "name" },
               new QueryArgument<IntGraphType> { Name = "priority" }
               ))
           .ResolveAsync(async context => {
               var groupFromUserId = context.GetArgument<Guid>("groupFromUserId");
               var nameGroup = context.GetArgument<string>("name");
               var priority = context.GetArgument<int?>("priority", null);

               var groupsFromUser = await groupsFromUserRepository.GetByIdAsync(groupFromUserId);

               groupsFromUser.Name = nameGroup ?? groupsFromUser.Name;
               groupsFromUser.Priority = priority ?? groupsFromUser.Priority;

               return await groupsFromUserRepository.UpdateAsync(groupsFromUser);
           });

        Field<BooleanGraphType>("deleteGroupFromUser")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupFromUserId" }
                ))
            .ResolveAsync(async context => {
                var groupFromUserId = context.GetArgument<Guid>("groupFromUserId");
               
                await groupsFromUserRepository.DeleteAsync(groupFromUserId);
                return true;
            });
    }
}
