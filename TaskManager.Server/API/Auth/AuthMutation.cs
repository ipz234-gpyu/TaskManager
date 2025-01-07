using GraphQL;
using GraphQL.Types;
using System.Security.Claims;
using TaskManager.Server.API.Auth.Models;
using TaskManager.Server.API.Auth.Types;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Services;
using TaskManager.Server.Domain.Errors;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.API.Auth;

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

        Field<AuthorizeResponseType>("authorize")
            .Arguments(new QueryArguments(new QueryArgument<StringGraphType> { Name = "refreshToken" }
            ))
            .ResolveAsync(async context =>
            {
                try
                {
                    var refreshToken = context.GetArgument<string>("refreshToken");

                    var principal = jwtTokenUtils.ValidateToken(refreshToken);

                    var refreshTokenHash = principal.FindFirst("hash")?.Value;

                    var tokenIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception();

                    if (!Guid.TryParse(tokenIdClaim, out var tokenId))
                        throw new Exception("Invalid TokenId format.");

                    var token = await tokenRepository.GetByIdAsync(tokenId) ?? throw new Exception();

                    var user = await userRepository.GetByIdAsync(token.UserId) ?? throw new Exception();

                    if (user == null || token.RefreshTokenHash != refreshTokenHash)
                        throw new Exception();

                    var newAccessToken = jwtTokenUtils.GenerateAccessToken(user.UserId);

                    return new AuthorizeResponse(
                        user,
                        newAccessToken
                    );
                }
                catch
                {
                    context.Errors.Add(ErrorCode.UNAUTHORIZED);
                    return null;
                }
            });

        Field<BooleanGraphType>("logout")
            .Arguments(new QueryArguments(
                new QueryArgument<StringGraphType> { Name = "refreshToken" }
            ))
            .ResolveAsync(async context =>
            {
                try
                {
                    var refreshToken = context.GetArgument<string>("refreshToken");

                    var principal = jwtTokenUtils.ValidateToken(refreshToken);

                    var tokenIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception();

                    if (!Guid.TryParse(tokenIdClaim, out var tokenId))
                        throw new Exception("Invalid TokenId format.");

                    var refreshTokenHash = principal.FindFirst("hash")?.Value;

                    await jwtTokenUtils.RevokeRefreshToken(tokenId, refreshTokenHash);

                    return true;
                }
                catch
                {
                    context.Errors.Add(ErrorCode.UNAUTHORIZED);

                    return null;
                }
            });
    }
}
