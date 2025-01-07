using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.GroupsFromUser.Types;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.GroupsFromUser;

public class GroupsFromUserQuery : ObjectGraphType
{
    public GroupsFromUserQuery(IGroupsFromUserRepository groupsFromUserRepository)
    {
        this.Authorize();

        Field<ListGraphType<GroupFromUserResponsType>>("getAllGroupFromUser")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" }
                ))
            .ResolveAsync(async context =>
            {
                var userId = context.GetArgument<Guid>("userId");
                return await groupsFromUserRepository.GetAllForUserAsync(userId);
            });

        Field<GroupFromUserResponsType>("getGroupFromUserById")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "groupFromUserId" }
                ))
            .ResolveAsync(async context =>
            {
                var groupFromUserId = context.GetArgument<Guid>("groupFromUserId");
                return await groupsFromUserRepository.GetByIdAsync(groupFromUserId);
            });
    }
}
