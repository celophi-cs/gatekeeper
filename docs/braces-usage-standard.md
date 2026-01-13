# Braces Usage Standard

## Standard
- Always use braces `{}` for all control flow statements (if, else, for, foreach, while, do, using, etc.), even for single-line statements.
- Do not rely on indentation or omit braces for brevity.
- Be explicit and consistent to improve readability and reduce errors.

## Rationale
- Prevents bugs caused by ambiguous or misleading indentation.
- Makes code changes safer and reviews clearer.
- Aligns with best practices for maintainable and explicit code.

## Examples

**Correct:**
```csharp
if (condition)
{
    DoSomething();
}
else
{
    DoSomethingElse();
}
```

**Incorrect:**
```csharp
if (condition)
    DoSomething();
else
    DoSomethingElse();
```

---

**See also:**
- [one-top-level-type-per-file-standard.md](one-top-level-type-per-file-standard.md)
- [test-organization-standard.md](test-organization-standard.md)
