using Gatekeeper.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<CosmosDbService>();
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

// Map endpoints
var authApi = new AuthApi(app.Services.GetRequiredService<CosmosDbService>());
var consentApi = new ConsentApi(app.Services.GetRequiredService<CosmosDbService>());

app.MapPost("/api/login", (HttpContext ctx, string email, string password) => authApi.Login(ctx, email, password));
app.MapPost("/api/logout", (HttpContext ctx) => authApi.Logout(ctx));

app.MapGet("/api/consent", (HttpContext ctx) => consentApi.GetConsent(ctx));
app.MapPost("/api/consent/approve", (HttpContext ctx, ConsentApprovalDto dto) => consentApi.ApproveConsent(ctx, dto));

app.UseAuthentication();
app.UseAuthorization();
app.Run();
