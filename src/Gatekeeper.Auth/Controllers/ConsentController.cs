using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Gatekeeper.Auth.Models;
using Gatekeeper.Auth.Extensions;

namespace Gatekeeper.Auth.Controllers
{
    [Route("consent")]
    public class ConsentController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;

        public ConsentController(IOpenIddictApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("OpenIddict request missing.");

            var client = await _applicationManager.FindByClientIdAsync(request.ClientId ?? string.Empty);
            if (client == null) return BadRequest("Unknown client.");

            var vm = new ConsentViewModel
            {
                ClientId = request.ClientId,
                ClientName = (await _applicationManager.GetDisplayNameAsync(client)) ?? request.ClientId,
                Scopes = request.GetScopes().AsEnumerable() ?? Enumerable.Empty<string>(),
                RequestId = request.RequestId
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Accept(string requestId, string[] scopes)
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("OpenIddict request missing.");

            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User missing subject.");

            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject, subject);

            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(scopes ?? Enumerable.Empty<string>().ToArray());

            await HttpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
