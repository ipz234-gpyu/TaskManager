using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Tags.Types;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Tags;

public class TagQuery : ObjectGraphType
{
    public TagQuery(ITagRepository tagRepository, IUserRepository userRepository, ITeamRepository teamRepository)
    {
        this.Authorize();

        Field<ListGraphType<TagResponsType>>("getAllTagsForUser")
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

            return await tagRepository.GetAllForUser(user.UserId);
           });

        Field<ListGraphType<TagResponsType>>("getAllTagsForTeam")
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

            return await tagRepository.GetAllForTeam(team.TeamId);
           });
    }
}
