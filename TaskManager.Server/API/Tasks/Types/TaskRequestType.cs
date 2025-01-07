using GraphQL.Types;

namespace TaskManager.Server.API.Tasks.Types;

public class TaskRequestType : InputObjectGraphType
{
    public TaskRequestType()
    {
        Field<GuidGraphType>("taskId");
        Field<NonNullGraphType<StringGraphType>>("title");
        Field<StringGraphType>("description");
        Field<DateTimeGraphType>("startTime");
        Field<DateTimeGraphType>("deadline");
        Field<DateTimeGraphType>("notification");
        Field<NonNullGraphType<IntGraphType>>("priority");
        Field<NonNullGraphType<StringGraphType>>("status");
        Field<GuidGraphType>("parentTaskId");
    }
}
