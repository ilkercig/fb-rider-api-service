namespace FbRider.Api.Responses;

public record TeamResponse(
    string Key,
    string Id,
    string Name,
    string? TeamLogo,
    IEnumerable<ManagerResponse> Managers,
    TeamRosterResponse? Roster
);

public record ManagerResponse(
    int Id,
    string Name,
    bool IsCurrentLogin,
    bool IsCommissioner
);

public record TeamRosterResponse(IEnumerable<PlayerResponse> Players);

public record PlayerResponse(
    string Key,
    string Id,
    string FullName,
    string? ImageUrl,
    string[] EligiblePositions,
    string[] DisplayPositions,
    string? SelectedPosition,
    bool IsUndroppable,
    string? Status
);
