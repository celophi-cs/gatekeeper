# One Top Level Type Per File Standard

## Standard
- Each file must contain only one top-level type (class, struct, record, interface, or enum).
- The file name must match the name of the top-level type it contains, including correct casing.
- Nested types are allowed, but only one top-level type is permitted per file.

## Rationale
- Improves code organization and discoverability.
- Simplifies navigation and refactoring.
- Reduces merge conflicts and improves source control history clarity.

## Examples

**Correct:**
- `UserService.cs` contains only the `UserService` class.
- `AuthResult.cs` contains only the `AuthResult` record.

**Incorrect:**
- `Models.cs` contains both `User` and `Role` classes.
- `Helpers.cs` contains multiple unrelated types.

## Exceptions
- Partial types may be split across multiple files, but each file must still contain only one top-level type (partial or otherwise).
- Auto-generated files may be excluded if required by tooling, but this should be rare and clearly documented.

---

**See also:**
- [test-organization-standard.md](test-organization-standard.md)
- [sealed-records-dto-standard.md](sealed-records-dto-standard.md)
