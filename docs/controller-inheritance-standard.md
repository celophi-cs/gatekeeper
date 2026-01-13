# Controller Inheritance Standard

## Standard
- API controllers must not inherit from `ControllerBase`.
- Controllers should be minimal and use static methods or endpoint mapping as needed.
- Avoid unnecessary inheritance to keep the codebase simple and explicit.

## Rationale
- Reduces unnecessary abstraction and dependencies.
- Encourages minimal, focused controller design.
- Aligns with modern endpoint mapping and minimal API patterns.

## Examples

**Correct:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController
{
    // Endpoint methods here (static or instance, but not inheriting ControllerBase)
}
```

**Incorrect:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    // ...
}
```

---

**See also:**
- [api-controller-standard.md](api-controller-standard.md)
- [braces-usage-standard.md](braces-usage-standard.md)
