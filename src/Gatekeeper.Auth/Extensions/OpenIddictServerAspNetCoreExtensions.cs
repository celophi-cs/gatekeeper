using Microsoft.AspNetCore.Http;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;

namespace Gatekeeper.Auth.Extensions
{
    public static class OpenIddictServerAspNetCoreExtensions
    {
        public static OpenIddictRequest? GetOpenIddictServerRequest(this HttpContext context)
        {
            return context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction?.Request;
        }
    }
}
