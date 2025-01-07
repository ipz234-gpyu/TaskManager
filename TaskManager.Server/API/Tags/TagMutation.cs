using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Tags.Types;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Tags;

public class TagMutation : ObjectGraphType
{
    public TagMutation(ITagRepository tagRepository, IUserRepository userRepository, ITaskRepository taskRepository)
    {
        this.Authorize();

        Field<TagResponsType>("createTag")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
               new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
               new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "color" }
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

            var tag = new Tag()
            {
                Name = context.GetArgument<string>("name"),
                Color = context.GetArgument<string>("color"),
                UserId = user.UserId
            };

            return await tagRepository.CreateAsync(tag);
        });

        Field<TagResponsType>("updateTag")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "tagId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
               new QueryArgument<StringGraphType> { Name = "name" },
               new QueryArgument<StringGraphType> { Name = "color" }
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

            var tagId = context.GetArgument<Guid>("tagId");
            var tag = await tagRepository.GetByIdAsync(tagId);
            if (tag == null || tag.UserId != user.UserId)
            {
                context.Errors.Add(ErrorCode.TAG_NOT_FOUND);
                return null;
            }

            tag.Name = context.GetArgument<string?>("name") ?? tag.Name;
            tag.Color = context.GetArgument<string?>("color") ?? tag.Color;

            return await tagRepository.UpdateAsync(tag);
        });

        Field<BooleanGraphType>("deleteTag")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "tagId" },
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

            var tagId = context.GetArgument<Guid>("tagId");
            var tag = await tagRepository.GetByIdAsync(tagId);
            if (tag == null || tag.UserId != user.UserId)
            {
                context.Errors.Add(ErrorCode.TAG_NOT_FOUND);
                return false;
            }

            await tagRepository.DeleteAsync(tag.TagId);
            return true;
        });

        Field<BooleanGraphType>("addTagToTask")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "tagId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "taskId" }
               ))
           .ResolveAsync(async context => {
               var tagId = context.GetArgument<Guid>("tagId");
               var tag = await tagRepository.GetByIdAsync(tagId);
               if (tag == null)
               {
                   context.Errors.Add(ErrorCode.TAG_NOT_FOUND);
                   return false;
               }

               var taskId = context.GetArgument<Guid>("taskId");
               var task = await taskRepository.GetByIdAsync(taskId);
               if (task == null)
               {
                   context.Errors.Add(ErrorCode.TASK_NOT_FOUND);
                   return false;
               }

               await tagRepository.CreateConnection(tag.TagId, task.TaskId);
               return true;
           });

        Field<BooleanGraphType>("removeTagFromTask")
           .Arguments(new QueryArguments(
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "tagId" },
               new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "taskId" }
               ))
           .ResolveAsync(async context => {
               var tagId = context.GetArgument<Guid>("tagId");
               var tag = await tagRepository.GetByIdAsync(tagId);
               if (tag == null)
               {
                   context.Errors.Add(ErrorCode.TAG_NOT_FOUND);
                   return false;
               }

               var taskId = context.GetArgument<Guid>("taskId");
               var task = await taskRepository.GetByIdAsync(taskId);
               if (task == null)
               {
                   context.Errors.Add(ErrorCode.TASK_NOT_FOUND);
                   return false;
               }

               await tagRepository.DeleteConnection(tag.TagId, task.TaskId);
               return true;
           });
    }
}
