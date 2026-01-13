# Mock Defaults Standard

## Purpose
Ensure all mocks in behavioral tests are set up with valid default values for flexibility and maintainability.

## Standard
- Every mock should have a 'happy path' default value set in the constructor.
- Use It.IsAny<> for all parameters in mock setups within the constructor.
- Bind default values using lambdas for deferred resolution.
- Store default values as class-level variables for easy manipulation in tests.
- Do not mix default setup with verification; keep them separate.

## Rationale
This reduces repetitive setup, enables easy scenario changes, and keeps tests simple and focused.