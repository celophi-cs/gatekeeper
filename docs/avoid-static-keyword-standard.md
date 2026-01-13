# Avoid Static Keyword Standard

## Standard
- Avoid using the `static` keyword for classes, methods, or members unless absolutely necessary.
- Prefer instance-based designs to improve testability, maintainability, and flexibility.
- Use dependency injection and interfaces instead of static helpers or utilities.

## Rationale
- Static code is difficult to test, mock, and extend.
- Excessive use of static members leads to tightly coupled and unmaintainable code.

## Exceptions
- Constants (`const` or `static readonly` fields) are allowed.
- Extension methods may require static classes.
- Utility methods that are truly stateless and have no dependencies may use static, but this should be rare and justified.

---

**See also:**
- [mock-defaults-standard.md](mock-defaults-standard.md)
- [test-organization-standard.md](test-organization-standard.md)
