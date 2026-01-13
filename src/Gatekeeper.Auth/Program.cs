
using Gatekeeper.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add Cosmos DB (local emulator) for Identity and OpenIddict
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseCosmos(
        builder.Configuration["Cosmos:AccountEndpoint"] ?? "https://localhost:8081", // default local emulator
        builder.Configuration["Cosmos:AccountKey"] ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5QpQ=", // default local emulator key
        databaseName: builder.Configuration["Cosmos:DatabaseName"] ?? "GatekeeperAuthDb"
    )
);

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();
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
app.MapPost("/api/register", async (UserManager<ApplicationUser> userManager, string email, string password) =>
{
    var user = new ApplicationUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    if (result.Succeeded)
        return Results.Ok();
    return Results.BadRequest(result.Errors);
});

app.MapPost("/api/login", async (SignInManager<ApplicationUser> signInManager, string email, string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, false, false);
    if (result.Succeeded)
        return Results.Ok();
    return Results.Unauthorized();
});

app.Run();
