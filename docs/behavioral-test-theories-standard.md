# Behavioral Test Theories Standard

## Purpose
Establish guidelines for using xUnit theories in behavioral tests.

## Standard
- Use theories only when test cases vary only by input data and are highly cohesive.
- Each theory case must have a meaningful English description, concatenated from the display name and the first InlineData parameter.
- Suppress unused parameter warnings using `_ = _ + ""` or compiler suppression pragmas.
- Avoid theories unless they improve documentation and readability.

## Rationale
Theories should be used to group similar, cohesive tests for documentation purposes, not for code reuse.