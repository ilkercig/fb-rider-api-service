namespace FbRider.Api.Domain.Models;

public class StatCategory
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string Abbreviation { get; init; }
    public required int SortOrder { get; init; }
    public bool IsOnlyDisplayStat { get; init; }
}