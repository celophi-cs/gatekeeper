# Sealed Records Standard for DTOs

## Guideline

**Use sealed records with `get; init;` properties for all Data Transfer Objects (DTOs) and request/response objects.**

### Rationale
- Sealed records provide immutability, value-based equality, and concise syntax.
- `get; init;` properties ensure objects are immutable after construction, improving safety and predictability.
- Sealing records prevents unintended inheritance and enforces clear data contracts.

### Examples

```csharp
public sealed record UserDto
{
    public string Name { get; init; }
    public int Age { get; init; }
}
```

### Exceptions
- Only deviate from this standard if a third-party library or framework requires a different structure.

---

**Summary:** Always use sealed records with `get; init;` for DTOs and request/response objects unless a third-party dependency requires otherwise.
