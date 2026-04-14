using System.Linq.Expressions;

namespace AttractionCatalog.Domain.Core.Attractions.Ports;

/// <summary>
/// Specification pattern — encapsulates a query predicate that can be translated
/// to a database query or applied in-memory. Lives in the domain as a port;
/// concrete implementations live in Infrastructure.
/// </summary>
public interface IQuerySpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}
