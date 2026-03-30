namespace Domain.Modules.CatalogSearch.Specifications;

/// <summary>
/// Query specification for pre-filtering at the persistence level.
/// Converts conditions to a predicate that can be evaluated against stored entities
/// before loading them into memory (performance optimization).
/// Applied conditionally — e.g., LocationQuerySpecification is used only when
/// SearchCriteria.NearArea is provided.
/// </summary>
/// <typeparam name="T">The entity type to filter.</typeparam>
public interface IQuerySpecification<T>
{
    /// <summary>
    /// Produces a predicate that can be used to filter entities at the data source level.
    /// </summary>
    Func<T, bool> ToExpression();
}
