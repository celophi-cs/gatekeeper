# Test Naming Standard

## Purpose
Establish a clear, consistent convention for naming behavioral tests to maximize readability and maintainability.

## Standard
- Name each test in plain English, describing the expected business behavior.
- Use the pattern:
  
  `<system> should <expected behavior> when <discriminating criteria>`
  
  - For API tests, use `<endpoint> should <expected behavior> when <discriminating criteria>`.
- Always use the word "should" after the system/endpoint name.
- The expected behavior must be a clear, finite expectation (avoid vague phrases).
- The "when" clause is optional, but include it for tests with discriminating criteria.
- Chain multiple criteria with "and" in the "when" clause.
- Avoid long "and" chains; break out into separate tests if needed.
- Each test should focus on a single, cohesive business behavior.

## Examples
- `AddOrderItem should return bad request when order is not found`
- `CreateOrder should store new order in the database correctly and return the correct order data`
- `SearchCustomers should return results when query is valid`

## Rationale
This convention ensures tests are easy to understand, review, and maintain, even for those unfamiliar with the codebase.