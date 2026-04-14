namespace AttractionCatalog.Domain.Common.Exceptions;

/// <summary>
/// Thrown when a requested domain entity does not exist.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException() { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { }
}
