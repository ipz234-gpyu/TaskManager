using GraphQL.Types;

namespace TaskManager.Server.API
{
    public class APISchema : Schema
    {
        public APISchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<RootQuery>();
            Mutation = serviceProvider.GetRequiredService<RootMutation>();
        }
    }
}
