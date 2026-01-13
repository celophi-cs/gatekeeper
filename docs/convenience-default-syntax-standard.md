# Convenience: Default Syntax Standard

## Purpose
Simplify mock setup in behavioral tests using concise default syntax.

## Standard
- Use custom fluent extensions like `.WithDefault(() => value)` to set default return values for mocks.
- Prefer default syntax over verbose `.Setup` methods when possible.
- Only fall back to verbose setup if necessary for complex scenarios.

## Rationale
Default syntax reduces boilerplate, prevents parameter expectation mistakes, and keeps tests clean and maintainable.