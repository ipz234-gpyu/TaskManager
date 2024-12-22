using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.GroupsFromUser.Types;
using TaskManager.Server.API.Teams.Types;
using TaskManager.Server.Infrastructure.Interfaces;
using TaskManager.Server.Infrastructure.Repositories;

namespace TaskManager.Server.API.Teams
{
    public class TeamQuery : ObjectGraphType
    {
        public TeamQuery(ITeamRepository teamRepository)
        {
            this.Authorize();

            Field<ListGraphType<TeamResponsType>>("getAllGroupFromUser")
                .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" }
                    ))
            .ResolveAsync(async context =>
            {
                    var userId = context.GetArgument<Guid>("userId");
                    return await teamRepository.GetAllByUserId(userId);
                });
        }
    }
}
