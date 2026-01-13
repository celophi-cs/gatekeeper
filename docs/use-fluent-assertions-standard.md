# Use Fluent Assertions Standard

## Purpose
Ensure all behavioral tests use FluentAssertions for clear, expressive, and maintainable assertions.

## Standard
- Use [FluentAssertions](https://fluentassertions.com/) for all assertions in behavioral tests.
- Write assertions in a way that reads like English and clearly communicates the expected outcome.
- Each assertion should be accompanied by a descriptive comment explaining the business expectation.
- Avoid using technical jargon in comments; focus on domain language.
- Inline all data being validated in the assertion for clarity, even if repetitive.
- Prefer simple, direct assertions over complex or abstracted validation methods.
- Do not reference setup classes or write common validation methods unless absolutely necessary for clarity.
- Tests should be "damp" (easy to read and understand), not "dry" (overly abstracted).

## Examples
```csharp
// then I expect the response to be OK
response.ShouldBeOk();

// then I expect the line item price for the movie to be
order.Items.Single().Price.Should().Be(expectedPrice);
```

## Rationale
Using FluentAssertions with clear, direct assertions makes behavioral tests serve as documentation for business behavior and protects against obfuscation.