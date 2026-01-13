# Test Organization Standard

## Purpose
Ensure behavioral tests are organized for clarity, maintainability, and coverage.

## Standard
- Create a single behavioral test project per web application.
- Organize test folders to match the structure of the API project (e.g., by controller).
- Each controller should have a dedicated folder.
- Each endpoint should have a dedicated specification file; never test more than one endpoint per file.
- For complex endpoints, create a folder for the endpoint and place multiple specification files inside.

## Rationale
This structure makes it easy to locate, review, and maintain tests, and ensures comprehensive coverage.