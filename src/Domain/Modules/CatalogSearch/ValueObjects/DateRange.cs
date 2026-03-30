namespace Domain.Modules.CatalogSearch.ValueObjects;

/// <summary>
/// Value Object representing a time range for availability queries.
/// Immutable pair of start/end dates.
/// </summary>
public sealed record DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateRange(DateTime start, DateTime end)
    {
        if (end < start)
            throw new ArgumentException("End date must be greater than or equal to start date.");

        Start = start;
        End = end;
    }

    /// <summary>
    /// Checks if a given DateTime falls within this range (inclusive).
    /// </summary>
    public bool Contains(DateTime dateTime) => dateTime >= Start && dateTime <= End;

    /// <summary>
    /// Checks if this range overlaps with another range.
    /// </summary>
    public bool Overlaps(DateRange other) => Start <= other.End && End >= other.Start;

    /// <summary>
    /// Returns the duration of the range.
    /// </summary>
    public TimeSpan Duration => End - Start;
}
