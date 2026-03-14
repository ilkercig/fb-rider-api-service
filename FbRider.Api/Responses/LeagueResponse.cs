namespace FbRider.Api.Responses;

public record LeagueResponse(
    string Key,
    string Id,
    string Name,
    string? LogoUrl,
    string ScoringType,
    int CurrentWeek,
    int StartWeek
);
