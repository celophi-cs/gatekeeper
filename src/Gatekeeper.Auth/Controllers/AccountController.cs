using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Gatekeeper.Auth.Controllers;

[ApiController]
[Route("[controller]")]
[ApiVersion("1.0")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto register)
    {
        var user = new IdentityUser { UserName = register.Email, Email = register.Email };
        var result = await _userManager.CreateAsync(user, register.Password);
        if (result.Succeeded)
            return Ok();
        var traceId = HttpContext.TraceIdentifier;
        var problem = new ProblemDetails
        {
            Status = 400,
            Title = "Registration failed",
            Detail = "User registration did not succeed.",
            Instance = HttpContext.Request.Path
        };
        problem.Extensions["errors"] = result.Errors;
        problem.Extensions["traceId"] = traceId;
        return BadRequest(problem);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user == null)
        {
            var traceId = HttpContext.TraceIdentifier;
            var problem = new ProblemDetails
            {
                Status = 400,
                Title = "Invalid credentials",
                Detail = "User not found.",
                Instance = HttpContext.Request.Path
            };
            problem.Extensions["traceId"] = traceId;
            return BadRequest(problem);
        }
        var result = await _signInManager.PasswordSignInAsync(user, login.Password, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
            return Ok();
        {
            var traceId = HttpContext.TraceIdentifier;
            var problem = new ProblemDetails
            {
                Status = 400,
                Title = "Invalid credentials",
                Detail = "Login failed.",
                Instance = HttpContext.Request.Path
            };
            problem.Extensions["traceId"] = traceId;
            return BadRequest(problem);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}

// DTOs (move to separate file if needed)
public sealed record LoginRequestDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed record RegisterRequestDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
