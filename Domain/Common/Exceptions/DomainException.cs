namespace AttractionCatalog.Domain.Common.Exceptions;

/// <summary>
/// Base exception for all domain-level rule violations.
/// </summary>
public class DomainException : Exception
{
    public DomainException() { }
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
