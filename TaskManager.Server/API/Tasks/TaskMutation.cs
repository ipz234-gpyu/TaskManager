using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Tasks.Types;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;
using Task = TaskManager.Server.Domain.Entities.Task;

namespace TaskManager.Server.API.Tasks;

public class TaskMutation : ObjectGraphType
{
    public TaskMutation(ITaskRepository taskRepository, IListRepository listRepository, 
        IUserRepository userRepository, ITeamRepository teamRepository)
    {
        this.Authorize();

        Field<TaskResponsType>("createTaskForList")
          .Arguments(new QueryArguments(
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" },
              new QueryArgument<NonNullGraphType<TaskRequestType>> { Name = "task" }
              ))
          .ResolveAsync(async context => {
              var listId = context.GetArgument<Guid>("listId");
              var list = await listRepository.GetByIdAsync(listId);
              if (list == null)
              {
                  context.Errors.Add(ErrorCode.GROUP_NOT_FOUND);
                  return null;
              }
              
              var task = context.GetArgument<Task>("task");
              if (task.Notification != null && task.Notification < DateTime.UtcNow.AddSeconds(-5))
              {
                  context.Errors.Add(ErrorCode.REMINDER_CANNOT_BE_IN_THE_PAST);
                  return null;
              }

              return await taskRepository.CreateForList(task, list.ListId);
          });

        Field<TaskResponsType>("updateTask")
          .Arguments(new QueryArguments(
              new QueryArgument<NonNullGraphType<TaskRequestType>> { Name = "task" }
              ))
          .ResolveAsync(async context => {
              var task = context.GetArgument<Task>("task");
              var oldTask = await taskRepository.GetByIdAsync(task.TaskId);
              if(oldTask == null)
              {
                  context.Errors.Add(ErrorCode.TASK_NOT_FOUND);
                  return null;
              }

              return await taskRepository.UpdateAsync(task);
          });

        Field<BooleanGraphType>("deleteTask")
          .Arguments(new QueryArguments(
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "taskId" }
              ))
          .ResolveAsync(async context => {
              var taskId = context.GetArgument<Guid>("taskId");
              var task = await taskRepository.GetByIdAsync(taskId);
              if(task == null)
              {
                  context.Errors.Add(ErrorCode.TASK_NOT_FOUND);
                  return false;
              }

              await taskRepository.DeleteAsync(taskId);
              return true;
          });

        Field<BooleanGraphType>("moveTaskTo")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "listId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "taskId" }
               ))
           .ResolveAsync(async context => {
               var listId = context.GetArgument<Guid>("listId");
               var list = await listRepository.GetByIdAsync(listId);
               if (list == null)
               {
                   context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                   return false;
               }

               var taskId = context.GetArgument<Guid>("taskId");
               var task = await taskRepository.GetByIdAsync(taskId);
               if (task == null)
               {
                   context.Errors.Add(ErrorCode.TASK_NOT_FOUND);
                   return false;
               }

               await taskRepository.CreateConnection(list.ListId, task.TaskId);

               var removedListId = context.GetArgument<Guid?>("removedListId");
               if (removedListId.HasValue)
               {
                   var removedList = await listRepository.GetByIdAsync(removedListId.Value);
                   if (removedList == null || list.ListId == removedList.ListId)
                   {
                       context.Errors.Add(ErrorCode.LIST_NOT_FOUND);
                       return false;
                   }
                   else await taskRepository.DeleteConnection(removedList.ListId, task.TaskId);
               }

               return true;
           });

        Field<BooleanGraphType>("assignTaskToTeamMember")
          .Arguments(new QueryArguments(
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" },
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "taskId" }
              ))
          .ResolveAsync(async context => {
              var userId = context.GetArgument<Guid>("userId");
              var user = await userRepository.GetByIdAsync(userId);
              if (user == null)
              {
                  context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                  return false;
              }

              var teamId = context.GetArgument<Guid>("teamId");
              var team = await teamRepository.GetByIdAsync(teamId);
              if (team == null)
              {
                  context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                  return false;
              }

              var taskId = context.GetArgument<Guid>("taskId");
              var task = await taskRepository.GetByIdAsync(taskId);
              if (task == null)
              {
                  context.Errors.Add(ErrorCode.TASK_NOT_FOUND);
                  return false;
              }

              await taskRepository.AssignTaskToTeamMember(user.UserId, team.TeamId, task.TaskId);
              return true;
          });

        Field<BooleanGraphType>("removeTaskFromTeamMember")
          .Arguments(new QueryArguments(
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "teamId" },
              new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "taskId" }
              ))
          .ResolveAsync(async context => {
              var userId = context.GetArgument<Guid>("userId");
              var user = await userRepository.GetByIdAsync(userId);
              if (user == null)
              {
                  context.Errors.Add(ErrorCode.USER_NOT_FOUND);
                  return false;
              }

              var teamId = context.GetArgument<Guid>("teamId");
              var team = await teamRepository.GetByIdAsync(teamId);
              if (team == null)
              {
                  context.Errors.Add(ErrorCode.TEAM_NOT_FOUND);
                  return false;
              }

              var taskId = context.GetArgument<Guid>("taskId");
              var task = await taskRepository.GetByIdAsync(taskId);
              if (task == null)
              {
                  context.Errors.Add(ErrorCode.TASK_NOT_FOUND);
                  return false;
              }

              await taskRepository.RemoveTaskFromTeamMember(user.UserId, team.TeamId, task.TaskId);
              return true;
          });
    }
}
