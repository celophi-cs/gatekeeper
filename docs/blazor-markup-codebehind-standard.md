# Blazor Markup and Code-Behind Standard

## Purpose
Ensure maintainability, readability, and separation of concerns in Blazor projects by splitting markup and logic.

## Standard
- All Blazor Razor pages and components must have their markup in a `.razor` file.
- All C# logic (the `@code` block) must be placed in a code-behind file with the `.razor.cs` extension.
- The code-behind file must use a partial class with the same name as the component/page.
- The `.razor` file should only contain markup and minimal `@using`/`@inject` directives.
- No business logic or methods should be present in the `.razor` file.

## Rationale
This separation improves code organization, testability, and makes it easier for teams to collaborate on UI and logic independently.

## Example

**Counter.razor**
```razor
@page "/counter"

<h3>Counter</h3>

<button @onclick="IncrementCount">Click me</button>
<p>Current count: @currentCount</p>
```

**Counter.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

public partial class Counter : ComponentBase
{
    private int currentCount = 0;
    private void IncrementCount() => currentCount++;
}
```

---

**Summary:** Always separate markup and logic for Blazor Razor pages and components using `.razor` and `.razor.cs` files.