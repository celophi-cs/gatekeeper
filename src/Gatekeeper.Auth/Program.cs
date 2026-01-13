using Gatekeeper.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.Configure<SeedDataOptions>(builder.Configuration.GetSection("SeedData"));
builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection("Cosmos"));
builder.Services.AddSingleton<ICosmosDbProvider, CosmosDbProvider>();
builder.Services.AddSingleton<CosmosDbSeeder>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddOpenIddict()
.AddCore(options =>
{
    options.Services.AddSingleton<IOpenIddictApplicationStore<Client>, CosmosApplicationStore>();
    options.Services.AddSingleton<IOpenIddictAuthorizationStore<Authorization>, CosmosAuthorizationStore>();
    options.Services.AddSingleton<IOpenIddictTokenStore<Token>, CosmosTokenStore>();
    options.Services.AddSingleton<IOpenIddictScopeStore<Scope>, CosmosScopeStore>();
})
.AddServer(options =>
{
    options.SetAuthorizationEndpointUris("/connect/authorize")
           .SetTokenEndpointUris("/connect/token")
           .AllowAuthorizationCodeFlow()
           .RequireProofKeyForCodeExchange();

    options.UseAspNetCore()
           .EnableAuthorizationEndpointPassthrough()
           .EnableTokenEndpointPassthrough();

    options.AddEphemeralEncryptionKey();
    options.AddEphemeralSigningKey();
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed Cosmos DB
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<CosmosDbSeeder>();
    await seeder.SeedAsync();
}

// Map endpoints
var authApi = new AuthApi(app.Services.GetRequiredService<ICosmosDbProvider>());
var consentApi = new ConsentApi(app.Services.GetRequiredService<ICosmosDbProvider>());

app.MapPost("/api/login", (HttpContext ctx, string email, string password) => authApi.Login(ctx, email, password));
app.MapPost("/api/logout", (HttpContext ctx) => authApi.Logout(ctx));

app.MapGet("/api/consent", (HttpContext ctx) => consentApi.GetConsent(ctx));
app.MapPost("/api/consent/approve", (HttpContext ctx, ConsentApprovalDto dto) => consentApi.ApproveConsent(ctx, dto));

app.UseAuthentication();
app.UseAuthorization();
app.Run();
