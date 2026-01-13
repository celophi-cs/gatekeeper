# API Controller Standard

## Standard
- API controllers must be placed in a `Controllers` folder.
- Controllers must use the `[ApiController]` attribute.
- Controllers must not contain business logic; delegate to services or other layers.

## Rationale
- Promotes clear project structure and separation of concerns.
- Ensures consistent use of ASP.NET Core conventions.

## Example

```csharp
// Controllers/UserController.cs
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    // ... delegate to services, no business logic here ...
}
```

---

**See also:**
- [test-organization-standard.md](test-organization-standard.md)
- [one-top-level-type-per-file-standard.md](one-top-level-type-per-file-standard.md)
