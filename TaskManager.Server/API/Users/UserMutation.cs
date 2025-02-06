using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Auth.Types;
using TaskManager.Server.API.Users.Types;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Services;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Users;

public class UserMutation : ObjectGraphType
{
    public UserMutation(IUserRepository userRepository)
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

        Field<LoginUserResponseType>("updateUser")
            .Authorize()
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
                new QueryArgument<StringGraphType> { Name = "newName" },
                new QueryArgument<StringGraphType> { Name = "newSurname" },
                new QueryArgument<StringGraphType> { Name = "newEmail" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                ))
            .ResolveAsync(async context =>
            {
                var password = context.GetArgument<string>("password");
                if (!DataValidator.IsPasswordValid(password))
                {
                    context.Errors.Add(ErrorCode.INVALID_PASSWORD_LENGTH);
                    return null;
                }

                var userId = context.GetArgument<Guid>("userId");
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null || !passwordHasher.VerifyHash(password, user.Password, user.Salt))
                {
                    context.Errors.Add(ErrorCode.INVALID_CREDENTIALS);
                    return null;
                }

                user.Name = context.GetArgument<string?>("newName") ?? user.Name;
                user.Surname = context.GetArgument<string?>("newSurname") ?? user.Surname;
                user.Email = context.GetArgument<string?>("newEmail") ?? user.Email;

                return await userRepository.UpdateAsync(user);
            });

        Field<BooleanGraphType>("deleteUser")
            .Authorize()
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<GuidGraphType>> { Name = "userId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
                ))
            .ResolveAsync(async context =>
            {
                var password = context.GetArgument<string>("password");
                if (!DataValidator.IsPasswordValid(password))
                {
                    context.Errors.Add(ErrorCode.INVALID_PASSWORD_LENGTH);
                    return false;
                }

                var userId = context.GetArgument<Guid>("userId");
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null || !passwordHasher.VerifyHash(password, user.Password, user.Salt))
                {
                    context.Errors.Add(ErrorCode.INVALID_CREDENTIALS);
                    return false;
                }

                await userRepository.DeleteAsync(userId);
                return true;
            });
    }
}
