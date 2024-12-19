using GraphQL;
using TaskManager.Server.API;
using TaskManager.Server.API.Helpers;
using TaskManager.Server.Application;
using TaskManager.Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddGraphQL(options => options
    .AddSchema<APISchema>()
    .AddSystemTextJson()
    .AddValidationRule<AuthorizationRule>()
    //.AddErrorInfoProvider(opt => opt.ExposeExceptionDetailsMode = GraphQL.Execution.ExposeExceptionDetailsMode.Message)
    .AddGraphTypes(typeof(APISchema).Assembly)
    .AddDataLoader());

// Add services to the container.

var app = builder.Build();

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors(builder => builder
    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.UseGraphQL<APISchema>();
app.UseGraphQLAltair();

app.MapControllers();

app.Run();