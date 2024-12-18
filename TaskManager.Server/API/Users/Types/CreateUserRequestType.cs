using GraphQL.Types;

namespace TaskManager.Server.API.Users.Types
{
    public class CreateUserRequestType : InputObjectGraphType
    {
        public CreateUserRequestType()
        {
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<StringGraphType>("surname");
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<NonNullGraphType<StringGraphType>>("password");
        }
    }
}
