using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Tags.Types;

public class TagResponsType : ObjectGraphType<Tag>
{
    public TagResponsType()
    {
        Field(t => t.TagId);
        Field(t => t.Name);
        Field(t => t.Color);
        Field(t => t.UserId);
    }
}
