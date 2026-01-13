using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Gatekeeper.Auth.Extensions;
using Gatekeeper.Auth.Models;

namespace Gatekeeper.Auth.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/consent")]
    public class ConsentController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsentController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{requestId}")]
        public IActionResult Get(string requestId)
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("OpenIddict request missing.");

            // Return client info + requested scopes to Blazor app
            return Ok(new
            {
                request.ClientId,
                ClientName = request.ClientId, // replace with DB lookup if needed
                Scopes = request.GetScopes(),
                RequestId = request.RequestId
            });
        }

        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromBody] ConsentApprovalDto dto)
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("OpenIddict request missing.");

            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            // Build claims for the consent
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject, User.FindFirstValue(ClaimTypes.NameIdentifier));
            identity.SetScopes(dto.Scopes); // Scopes user approved

            var principal = new ClaimsPrincipal(identity);

            // Sign in to complete the consent flow
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
