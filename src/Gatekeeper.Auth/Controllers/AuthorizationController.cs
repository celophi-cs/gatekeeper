using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Gatekeeper.Auth.Controllers;

[ApiController]
[Route("connect/[controller]")]
[ApiVersion("1.0")]
public class AuthorizeController : ControllerBase
{
    [HttpPost]
    public IActionResult Authorize()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }
        // TODO: Implement consent logic as needed
        return Ok();
    }
}
