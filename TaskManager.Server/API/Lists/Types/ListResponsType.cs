using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Lists.Types;

public class ListResponsType : ObjectGraphType<List>
{
    public ListResponsType()
    {
        Field(t => t.ListId);
        Field(t => t.Name);
        Field(t => t.Priority);
    }
}
