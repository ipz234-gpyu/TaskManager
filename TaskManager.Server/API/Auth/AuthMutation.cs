using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.Auth.Models;
using TaskManager.Server.API.Auth.Types;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Services;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Auth
{
    public class AuthMutation : ObjectGraphType
    {
        public AuthMutation(
            IUserRepository userRepository,
            IUserRefreshTokenRepository tokenRepository,
            IJwtTokenUtils jwtTokenUtils,
            IPasswordHasher passwordHasher
            )
        {
            Field<LoginResponseType>("login")
                .Arguments(new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "deviceInfo" },
                    new QueryArgument<StringGraphType> { Name = "email" },
                    new QueryArgument<StringGraphType> { Name = "password" }
                ))
                .ResolveAsync(async context =>
                {
                    var email = context.GetArgument<string>("email");
                    if (!DataValidator.IsEmailValid(email))
                    {
                        context.Errors.Add(ErrorCode.INVALID_EMAIL_FORMAT);
                        return null;
                    }

                    var password = context.GetArgument<string>("password");
                    if (!DataValidator.IsPasswordValid(password))
                    {
                        context.Errors.Add(ErrorCode.INVALID_PASSWORD_LENGTH);
                        return null;
                    }

                    var user = await userRepository.GetUserByEmailAsync(email);
                    if (user == null || !passwordHasher.VerifyHash(password, user.Password, user.Salt))
                    {
                        context.Errors.Add(ErrorCode.INVALID_CREDENTIALS);
                        return null;
                    }

                    var deviceInfo = context.GetArgument<string>("deviceInfo");
                    var accessToken = jwtTokenUtils.GenerateAccessToken(user.UserId);
                    var refreshToken = await jwtTokenUtils.GenerateRefreshToken(user.UserId, deviceInfo);

                    return new LoginResponse(
                       user,
                       accessToken,
                       refreshToken
                   );
                });
        }
    }
}
