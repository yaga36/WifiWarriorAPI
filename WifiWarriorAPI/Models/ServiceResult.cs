namespace WifiWarriorAPI.Models;

/// <summary>
/// Represents the outcome of a service-layer operation.
/// </summary>
/// <typeparam name="T">
/// The payload type returned when the operation succeeds. Use <see langword="object"/> when no payload is required.
/// </typeparam>
/// <param name="Success">
/// Indicates whether the operation completed successfully.
/// </param>
/// <param name="Value">
/// The result payload for successful operations; otherwise <see langword="null"/>.
/// </param>
/// <param name="Error">
/// A human-readable error message for failed operations; otherwise <see langword="null"/>.
/// </param>
/// <param name="StatusCode">
/// An optional status hint for the caller (for example, 400, 404, 409, 500).
/// Controllers may use this to map to appropriate HTTP responses.
/// </param>
public record ServiceResult<T>(
    bool Success,
    T? Value = default,
    string? Error = null,
    int? StatusCode = null);