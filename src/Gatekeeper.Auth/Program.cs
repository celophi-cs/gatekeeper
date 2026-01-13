
using Gatekeeper.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Bind Cosmos options from config
builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection("Cosmos"));

// Add Cosmos DB (local emulator) for Identity and OpenIddict using IOptions pattern
builder.Services.AddDbContext<AuthDbContext>((serviceProvider, options) =>
{
    var cosmosOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CosmosOptions>>().Value;
    options.UseCosmos(
        cosmosOptions.AccountEndpoint,
        cosmosOptions.AccountKey,
        cosmosOptions.DatabaseName
    );
});

// Add ASP.NET Core Identity (default IdentityUser/IdentityRole)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Add OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AuthDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/connect/token");
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
        options.AllowRefreshTokenFlow();
        options.AcceptAnonymousClients();
        options.AddDevelopmentEncryptionCertificate();
        options.AddDevelopmentSigningCertificate();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Minimal API endpoints for registration and login (to be implemented)
app.MapPost("/api/register", async (UserManager<IdentityUser> userManager, string email, string password) =>
{
    var user = new IdentityUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    if (result.Succeeded)
        return Results.Ok();
    return Results.BadRequest(result.Errors);
});

app.MapPost("/api/login", async (SignInManager<IdentityUser> signInManager, string email, string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, false, false);
    if (result.Succeeded)
        return Results.Ok();
    return Results.Unauthorized();
});

app.Run();
