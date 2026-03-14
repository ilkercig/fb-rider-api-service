namespace FbRider.Api.Responses;

public record SeasonResponse(
    string Key,
    int SeasonYear,
    bool IsSeasonOver,
    IEnumerable<LeagueResponse> Leagues
);
