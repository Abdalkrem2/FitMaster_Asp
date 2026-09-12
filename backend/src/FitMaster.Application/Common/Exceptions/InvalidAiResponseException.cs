namespace FitMaster.Application.Common.Exceptions;

/// <summary>
/// Thrown when an AI-generated response (meal plan JSON, etc.) is missing, malformed,
/// or fails structural validation after all retries are exhausted. Maps to HTTP 500
/// via GlobalExceptionHandler - an AI provider failure isn't a normal client-facing
/// business outcome, so it doesn't go through Result.
/// </summary>
public class InvalidAiResponseException(string message) : Exception(message);
