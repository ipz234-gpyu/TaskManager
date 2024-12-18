using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Users.Types;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Services;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Users
{
    public class UserMutation : ObjectGraphType
    {
        public UserMutation(IUserRepository userRepository,
            IUserRefreshTokenRepository userRefreshTokenRepository)
        {
            Field<BooleanGraphType>("createUser")
                .Arguments(new QueryArguments(new QueryArgument<CreateUserRequestType> { Name = "user" }))
                .ResolveAsync(async context =>
                {
                    var userInput = context.GetArgument<User>("user");

                    if (userInput == null)
                    {
                        context.Errors.Add(ErrorCode.INVALID_INPUT_DATA);
                        return null;
                    }

                    if (!DataValidator.IsEmailValid(userInput.Email))
                    {
                        context.Errors.Add(ErrorCode.INVALID_EMAIL_FORMAT);
                        return null;
                    }

                    if (await userRepository.GetUserByEmailAsync(userInput.Email) != null)
                    {
                        context.Errors.Add(ErrorCode.EMAIL_EXIST);
                        return null;
                    }

                    var passwordHasher = context.RequestServices.GetRequiredService<IPasswordHasher>();
                    var hashPasswordResponse = passwordHasher.HashPassword(userInput.Password);

                    var user = new User()
                    {
                        Name = userInput.Name,
                        Surname = userInput.Surname,
                        Email = userInput.Email,
                        Password = hashPasswordResponse.Password,
                        Salt = hashPasswordResponse.Salt
                    };

                    return await userRepository.CreateAsync(user) != null;
                });
        }
    }
}
