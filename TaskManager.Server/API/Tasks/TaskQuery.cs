using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Tasks.Types;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Tasks;

public class TaskQuery : ObjectGraphType
{
    public TaskQuery(ITaskRepository taskRepository, IListRepository listRepository,
        IUserRepository userRepository, ITeamRepository teamRepository)
    {
        this.Authorize();

        Field<ListGraphType<TaskResponsType>>("getAllTasksForList")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" }
               ))
           .ResolveAsync(async context =>
           {
               var listId = context.GetArgument<Guid>("listId");
               var list = await listRepository.GetByIdAsync(listId);
               if (list == null)
               {
                   context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                   return null;
               }

               return await taskRepository.GetAllForList(list.ListId);
           });

        Field<ListGraphType<TaskResponsType>>("getUserTasksInDateRange")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
               new QueryArgument<NonNullGraphType<DateTimeGraphType>> { Name = "startTime" },
               new QueryArgument<NonNullGraphType<DateTimeGraphType>> { Name = "endTime" }
               ))
           .ResolveAsync(async context =>
           {
               var userId = context.GetArgument<Guid>("userId");
               var user = await userRepository.GetByIdAsync(userId);
               if (user == null)
               {
                   context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                   return null;
               }

               var startTime = context.GetArgument<DateTime>("startTime");
               var endTime = context.GetArgument<DateTime>("endTime");

               return await taskRepository.GetUserTasksInDateRange(user.UserId, startTime, endTime);
           });

        Field<ListGraphType<TaskResponsType>>("getTeamTasksInDateRange")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" },
               new QueryArgument<NonNullGraphType<DateTimeGraphType>> { Name = "startTime" },
               new QueryArgument<NonNullGraphType<DateTimeGraphType>> { Name = "endTime" }
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

               var startTime = context.GetArgument<DateTime>("startTime");
               var endTime = context.GetArgument<DateTime>("endTime");

               return await taskRepository.GetUserTasksInDateRange(team.TeamId, startTime, endTime);
           });

        Field<ListGraphType<TaskResponsType>>("getUserTasksByTags")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
               new QueryArgument<ListGraphType<GuidGraphType>> { Name = "tagIds" }
               ))
           .ResolveAsync(async context =>
           {
               var userId = context.GetArgument<Guid>("userId");
               var user = await userRepository.GetByIdAsync(userId);
               if (user == null)
               {
                   context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                   return null;
               }

               var tagIds = context.GetArgument<IEnumerable<Guid>>("tagIds");

               return await taskRepository.GetUserTasksByTags(user.UserId, tagIds);
           });

        Field<ListGraphType<TaskResponsType>>("getTeamTasksByTags")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" },
               new QueryArgument<ListGraphType<GuidGraphType>> { Name = "tagIds" }
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

               var tagIds = context.GetArgument<IEnumerable<Guid>>("tagIds");

               return await taskRepository.GetUserTasksByTags(team.TeamId, tagIds);
           });
    }
}
