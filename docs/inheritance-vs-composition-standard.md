# Inheritance vs Composition Standard

## Guideline

**Avoid inheritance and always prefer composition, except where a third-party library or framework requires inheritance.**

### Rationale
- Composition provides greater flexibility, testability, and maintainability.
- Inheritance can introduce tight coupling and fragile base class problems.
- Only use inheritance when it is explicitly required by a third-party library or framework (e.g., base classes in ASP.NET, Entity Framework, or test frameworks).

### Examples
- Use dependency injection, interfaces, and aggregation to share behavior.
- Do not create custom base classes for internal code organization.
- If a third-party library requires you to inherit from a specific class, document the reason in code comments.

### Exceptions
- Inheritance is permitted only when a third-party library or framework mandates it for correct operation.

---

**Summary:** Favor composition over inheritance in all code, unless a third-party dependency requires inheritance.
