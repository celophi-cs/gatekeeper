using Microsoft.AspNetCore.Components;

namespace Gatekeeper.Demo.Client.Pages.Connect;

public partial class Authorize : ComponentBase
{
    private string ClientName { get; set; } = "Demo Client";
    private List<string> RequestedScopes { get; set; } = new() { "openid", "profile", "email" };

    private Task GrantAsync()
    {
        // TODO: Implement consent grant logic
        return Task.CompletedTask;
    }
    private Task DenyAsync()
    {
        // TODO: Implement consent deny logic
        return Task.CompletedTask;
    }
}
