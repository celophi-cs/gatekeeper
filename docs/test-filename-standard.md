# Test Filename Standard

## Standard
- Test files must use the suffix `Specification` (e.g., `ConnectAuthorizeEndpointSpecification.cs`).
- Each API endpoint must have its own specification file.

## Rationale
- Consistent naming makes tests easy to find and map to endpoints.
- Suffix `Specification` clarifies purpose and supports test discovery.

## Examples
- `ConnectAuthorizeEndpointSpecification.cs` for `/connect/authorize` endpoint.
- `UserLoginEndpointSpecification.cs` for `/api/login` endpoint.
