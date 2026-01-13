using System;

namespace Gatekeeper.Auth.Exceptions;

/// <summary>
/// Represents an API-level error that should be returned to the client as HTTP 400 (Bad Request).
/// Use this for validation-like or predictable client errors.
/// </summary>
public sealed class GatekeeperApiException : Exception
{
    public GatekeeperApiException(string message) : base(message) { }

    public GatekeeperApiException(string message, Exception innerException) : base(message, innerException) { }
}
