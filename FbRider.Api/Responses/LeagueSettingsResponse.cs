namespace FbRider.Api.Responses;

public record LeagueSettingsResponse(
    IEnumerable<StatCategoryResponse> StatCategories,
    IEnumerable<RosterPositionResponse> RosterPositions
);

public record StatCategoryResponse(
    int Id,
    string Name,
    string DisplayName,
    string Abbreviation,
    int SortOrder,
    bool IsOnlyDisplayStat
);

public record RosterPositionResponse(
    string Position,
    int Count,
    bool IsStartingPosition
);
