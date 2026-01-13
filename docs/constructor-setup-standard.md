# Constructor Setup Standard

## Purpose
Define best practices for setting up behavioral test specification files.

## Standard
- Use the constructor for common setup code that runs before every test.
- Create an application builder using Microsoft TestServer and WebApplicationFactory.
- Mock only the wire dependencies traversed by the code path, plus any that cross process boundaries.
- Store each mock as a class-level variable, named with a 'Mock' suffix.
- Set up a working 'happy path' default value for each mock, bound using a lambda for deferred resolution.

## Rationale
This approach minimizes test setup cost, maximizes flexibility, and keeps tests clean and maintainable.