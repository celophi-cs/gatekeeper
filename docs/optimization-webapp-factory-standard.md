# Optimization: Web Application Factory Standard

## Purpose
Describe how to optimize behavioral test performance by reusing the WebApplicationFactory.

## Standard
- Use xUnit class fixtures to reuse WebApplicationFactory across tests in a class.
- Inject the fixture into the constructor and capture the HttpClient as a class-level variable.
- Mock all dependencies up front in the constructor; do not mock new dependencies in individual tests.
- Ensure the fixture resets mocks for each test to avoid shared state issues.

## Rationale
This optimization increases test speed for large suites while maintaining isolation and correctness.