namespace FbRider.Api.Domain.Models;

public class AllPlayScore
{ 
    public required string TeamKey { get; init; }
    /// <summary>
    /// StatCategoryId -> Score
    /// </summary>
    public required Dictionary<int, float> StatScores { get; init; }
}