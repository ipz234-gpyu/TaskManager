using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManager.Server.Domain.Errors;

namespace TaskManager.Server.API.Helpers;

public class CustomAuthorizationVisitor : AuthorizationVisitor
{
    public CustomAuthorizationVisitor(ValidationContext context, ClaimsPrincipal claimsPrincipal, IAuthorizationService authorizationService)
        : base(context, claimsPrincipal, authorizationService)
    {

    }

    protected override void HandleNodeNotAuthorized(ValidationInfo info)
    {
        if (!info.Context.HasErrors)
            info.Context.ReportError(ErrorCode.UNAUTHORIZED);
    }
}
