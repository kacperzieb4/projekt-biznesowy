namespace Domain.Modules.CatalogSearch.Specifications;

/// <summary>
/// Specification pattern (DDD) — encapsulates complex business rules as composable,
/// testable predicates. Eliminates procedural if/else chains and supports
/// Open/Closed Principle: new rules are added by creating new specifications,
/// not by modifying existing code.
/// </summary>
/// <typeparam name="T">The type of candidate being evaluated.</typeparam>
public interface ISpecification<in T>
{
    /// <summary>
    /// Evaluates whether the given candidate satisfies all conditions
    /// encapsulated by this specification.
    /// </summary>
    bool IsSatisfiedBy(T candidate);
}
