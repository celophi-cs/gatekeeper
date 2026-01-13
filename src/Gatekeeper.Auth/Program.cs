
using Gatekeeper.Auth.Data;
using Gatekeeper.Auth.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and API versioning
builder.Services.AddControllers(options =>
{
    options.Filters.Add<Gatekeeper.Auth.Filters.ProblemDetailsExceptionFilterAttribute>();
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("api-version");
});

// Add authorization services (required for UseAuthorization middleware)
builder.Services.AddAuthorization();



// Determine DB provider: use InMemory for tests, Cosmos for normal runs
var useInMemory = Environment.GetEnvironmentVariable("USE_INMEMORY_DB") == "1";

if (useInMemory)
{
    builder.Services.AddDbContext<AuthDbContext>(options =>
    {
        options.UseInMemoryDatabase("TestDb");
    });
}
else
{
    // Bind Cosmos options from config
    builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection("Cosmos"));
    builder.Services.AddDbContext<AuthDbContext>((serviceProvider, options) =>
    {
        var cosmosOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CosmosOptions>>().Value;
        options.UseCosmos(
            cosmosOptions.AccountEndpoint,
            cosmosOptions.AccountKey,
            cosmosOptions.DatabaseName
        );
    });
}

// Bind OpenIddict options from config
builder.Services.Configure<OpenIddictOptions>(builder.Configuration.GetSection("OpenIddict"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AuthDbContext>();
    })
    .AddServer(options =>
    {
        // Use strongly-typed options for configuration
        options.Services.AddOptions<OpenIddict.Server.OpenIddictServerOptions>()
            .Configure<IServiceProvider, Microsoft.Extensions.Options.IOptions<Gatekeeper.Auth.Configuration.OpenIddictOptions>>((serverOptions, sp, config) =>
            {
                var oidc = config.Value;
                serverOptions.Issuer = new Uri(oidc.Issuer);
                // Use symmetric signing key from config (for demo only; use certificate in production)
                serverOptions.SigningCredentials.Add(
                    new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(oidc.SigningKey)),
                        Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
                );
            });
        // Add ephemeral encryption and signing keys for dev/test
        options.AddEphemeralEncryptionKey();
        options.AddEphemeralSigningKey();
        options.SetTokenEndpointUris("/connect/token");
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
        options.AllowRefreshTokenFlow();
        options.AcceptAnonymousClients();
        // For tests/dev only: allow HTTP (OpenIddict blocks non-HTTPS by default)
        // Note: some OpenIddict versions do not expose DisableTransportSecurityRequirement()
        // so this call is omitted to maintain compatibility.
        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Add OpenAPI/Swagger for API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Seed OpenIddict clients at startup (skip in-memory/test runs)
if (!useInMemory)
{
    using (var scope = app.Services.CreateScope())
    {
        await OpenIddictSeeder.SeedClientsAsync(scope.ServiceProvider);
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global ProblemDetails middleware
app.UseMiddleware<Gatekeeper.Auth.Middleware.ProblemDetailsMiddleware>();


app.UseHttpsRedirection();
app.UseAuthentication();

// Ensure unauthenticated /connect/authorize requests return 401 before OpenIddict validation
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/connect/authorize") && !(context.User?.Identity?.IsAuthenticated ?? false))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }
    await next();
});

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map minimal endpoints (register/login)
Gatekeeper.Auth.AuthEndpoints.MapAuthEndpoints(app);

app.Run();
