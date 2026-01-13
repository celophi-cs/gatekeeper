using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gatekeeper.Demo.Client.Pages.Account;

public partial class Login : ComponentBase
{
    private string Email { get; set; }
    private string Password { get; set; }
    private string ErrorMessage { get; set; }

    private async Task LoginAsync()
    {
        // TODO: Call Gatekeeper.Auth API for login
        ErrorMessage = "Demo: Implement login logic.";
    }
}
