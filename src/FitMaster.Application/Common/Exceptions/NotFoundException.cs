namespace FitMaster.Application.Common.Exceptions;

/// <summary>Thrown when a requested entity doesn't exist. Maps to HTTP 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"\"{entityName}\" ({key}) was not found.")
    {
    }
}
