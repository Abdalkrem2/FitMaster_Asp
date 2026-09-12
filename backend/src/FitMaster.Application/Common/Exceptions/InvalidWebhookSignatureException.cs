namespace FitMaster.Application.Common.Exceptions;


/// <summary>Thrown when a payment gateway webhook's signature doesn't verify against
/// the configured secret - maps to HTTP 400 in GlobalExceptionHandler. This is the
/// only thing standing between the (unauthenticated, by necessity) webhook endpoint
/// and anyone who isn't actually the gateway.</summary>
public class InvalidWebhookSignatureException : Exception
{
    public InvalidWebhookSignatureException(string message) : base(message)
    {
    }
}
