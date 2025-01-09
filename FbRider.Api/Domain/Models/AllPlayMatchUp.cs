namespace FbRider.Api.Domain.Models;

public class AllPlayMatchUp
{
    private MatchUpResult? _matchUpResult;
    public required TeamStats Team1 { get; init; }
    public required TeamStats Team2 { get; init; }

    public MatchUpResult MatchUpResult
    {
        get
        {
            if (_matchUpResult != null) return _matchUpResult;

            _matchUpResult = new MatchUpResult();
            foreach (var team1Stat in Team1.Stats)
            {
                var team2Stat = Team2.Stats.SingleOrDefault(s => s.CategoryId == team1Stat.CategoryId);
                if (team2Stat == null) throw new ArgumentException("Stat categories don't match.", nameof(Team2));

                if (team1Stat.CompareTo(team2Stat) == 1)
                    _matchUpResult.CategoriesTeam1Won.Add(team1Stat.CategoryId);
                else if (team1Stat.CompareTo(team2Stat) == -1)
                    _matchUpResult.CategoriesTeam2Won.Add(team1Stat.CategoryId);
                else
                    _matchUpResult.CategoriesTie.Add(team1Stat.CategoryId);
            }

            return _matchUpResult;
        }
    }
}
public class MatchUpResult
{
    public List<int> CategoriesTeam1Won { get; } = [];
    public List<int> CategoriesTeam2Won { get; } = [];
    public List<int> CategoriesTie { get; } = [];
}