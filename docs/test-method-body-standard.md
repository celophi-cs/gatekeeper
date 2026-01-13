# Test Method Body Standard

## Purpose
Define the structure and conventions for behavioral test method bodies.

## Standard
- Divide each test into three sections: Given, When, Then.
- 'Given' sets up class-level variables for the expected behavior.
- 'When' describes the action invoked on the SUT (system under test); only one 'when' per test.
- 'Then' contains one or more assertions, each with a descriptive English comment.
- Every test must have at least one assertion.

## Rationale
This structure makes tests readable, maintainable, and ensures clear documentation of business behavior.