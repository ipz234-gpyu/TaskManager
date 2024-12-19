using GraphQL.Validation;
using GraphQL;
using Microsoft.AspNetCore.Authorization;
using GraphQL.Server.Transports.AspNetCore;

namespace TaskManager.Server.API.Helpers
{
    public class AuthorizationRule : AuthorizationValidationRule
    {
        public override async ValueTask<INodeVisitor?> GetPreNodeVisitorAsync(ValidationContext context)
        {
            var user = context.User
                ?? throw new InvalidOperationException("User could not be retrieved from ValidationContext. Please be sure it is set in ExecutionOptions.User.");
            var provider = context.RequestServices
                ?? throw new MissingRequestServicesException();
            var authService = provider.GetService<IAuthorizationService>()
                ?? throw new InvalidOperationException("An instance of IAuthorizationService could not be pulled from the dependency injection framework.");

            var visitor = new CustomAuthorizationVisitor(context, user, authService);
            return await visitor.ValidateSchemaAsync(context) ? visitor : null;
        }
    }
}
