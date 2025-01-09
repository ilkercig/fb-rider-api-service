namespace FbRider.Api.Domain.Models;

public class TeamStats
{
    public required string TeamKey { get; init; }
    public required IEnumerable<IComparableStat> Stats { get; init; }
    public IStatValue<string>[]? DisplayStats { get; set; }
}

public class PositiveStat : ComparableStatBase
{
}

public class NegativeStat : ComparableStatBase
{
    public override int CompareTo(IComparableStat? other)
    {
        return -1 * base.CompareTo(other);
    }
}

public abstract class ComparableStatBase : IComparableStat
{
    public virtual int CompareTo(IComparableStat? other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (CategoryId != other.CategoryId)
            throw new ArgumentException("Stat categories don't match.", nameof(other));

        return Value.CompareTo(other.Value);
    }

    public int CategoryId { get; init; }
    public required float Value { get; init; }
}

public interface IComparableStat : IComparable<IComparableStat>, IStatValue<float>
{
}

public interface IStatValue<T>
{
    public int CategoryId { get; init; }
    public T Value { get; init; }
}

public class DisplayStatValue : IStatValue<string>
{
    public required int CategoryId { get; init; }
    public required string Value { get; init; }
}