# Test Request and Response Standard

## Purpose
Define conventions for request and response objects in behavioral tests.

## Standard
- Create dedicated test classes for every viewmodel used in requests and responses.
- Name test classes with a 'Test' prefix (e.g., TestAddOrderItemRequestViewModel).
- Do not use production viewmodel classes directly in tests.
- For enums, use strings or equivalent test enums in test viewmodels.

## Rationale
This protects against contract-breaking changes in production code and ensures tests validate external contracts, not internal objects.