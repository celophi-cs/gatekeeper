using Gatekeeper.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Core services
// ==========================
builder.Services.Configure<SeedDataOptions>(
    builder.Configuration.GetSection("SeedData"));

builder.Services.Configure<CosmosOptions>(
    builder.Configuration.GetSection("Cosmos"));

builder.Services.AddSingleton<ICosmosDbProvider, CosmosDbProvider>();
builder.Services.AddSingleton<CosmosDbSeeder>();

// ==========================
// Authentication
// ==========================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });

builder.Services.AddAuthorization();

// ==========================
// OpenIddict
// ==========================
builder.Services.AddOpenIddict()

    // 🔑 CORE (stores + entities)
    .AddCore(options =>
    {
        // Set default application and scope entities
        options.SetDefaultApplicationEntity<Client>();
        options.SetDefaultScopeEntity<Scope>();

        // Only register the ONE store we need right now
        options.Services.AddSingleton<
            IOpenIddictApplicationStore<Client>,
            CosmosApplicationStore>();
        // Register CosmosScopeStore as the default scope store
        options.Services.AddSingleton<
            IOpenIddictScopeStore<Scope>,
            CosmosScopeStore>();
    })

    // 🔑 SERVER
    .AddServer(options =>
    {
        options
            .SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token");

        options
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange();

        options
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey();

        options
            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough();
    });

// ==========================
// Build app
// ==========================
var app = builder.Build();

// ==========================
// Middleware (ORDER MATTERS)
// ==========================
app.UseAuthentication();
app.UseAuthorization();

// ==========================
// Seed Cosmos DB
// ==========================
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<CosmosDbSeeder>();
    await seeder.SeedAsync();
}

// ==========================
// APIs
// ==========================
var authApi = new AuthApi(app.Services.GetRequiredService<ICosmosDbProvider>());
var consentApi = new ConsentApi(app.Services.GetRequiredService<ICosmosDbProvider>());

app.MapPost("/api/login",
    (HttpContext ctx, string email, string password)
        => authApi.Login(ctx, email, password));

app.MapPost("/api/logout",
    (HttpContext ctx)
        => authApi.Logout(ctx));

app.MapGet("/api/consent",
    (HttpContext ctx)
        => consentApi.GetConsent(ctx));

app.MapPost("/api/consent/approve",
    (HttpContext ctx, ConsentApprovalDto dto)
        => consentApi.ApproveConsent(ctx, dto));

app.Run();
