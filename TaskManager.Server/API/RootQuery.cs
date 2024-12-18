using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Users.Types;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
        }
    }
}
