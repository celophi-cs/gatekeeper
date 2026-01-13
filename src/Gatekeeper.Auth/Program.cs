
using Gatekeeper.Auth.Data;
using Gatekeeper.Auth.Configuration;
using Gatekeeper.Auth.Endpoints;
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

// Seed OpenIddict clients at startup
using (var scope = app.Services.CreateScope())
{
    await OpenIddictSeeder.SeedClientsAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


// Register authentication endpoints via extension method
app.MapAuthEndpoints();

app.Run();
