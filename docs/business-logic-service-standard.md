# Business Logic Service Standard

## Standard
- All business logic must be implemented in classes suffixed with `Service` (e.g., `UserService`).
- Every service class must have a corresponding interface (e.g., `IUserService`).
- Controllers and endpoints must delegate business logic to these service classes via their interfaces.

## Rationale
- Enforces separation of concerns and testability.
- Promotes consistent naming and discoverability.
- Makes dependency injection and mocking straightforward.

## Examples

**Correct:**
```csharp
public interface IUserService
{
    Task<User> GetUserAsync(string id);
}

public class UserService : IUserService
{
    public Task<User> GetUserAsync(string id)
    {
        // Business logic here
    }
}
```

**Incorrect:**
```csharp
public class UserLogic
{
    // ...
}

// Or business logic directly in controllers
```

---

**See also:**
- [controller-inheritance-standard.md](controller-inheritance-standard.md)
- [one-top-level-type-per-file-standard.md](one-top-level-type-per-file-standard.md)
