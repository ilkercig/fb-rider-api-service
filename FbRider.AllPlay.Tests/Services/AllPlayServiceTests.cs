using FbRider.Api.Domain.Models;
using FbRider.Api.Services;
using FbRider.YahooApi;
using Moq;
using FbRider.AllPlay.Domain.Services;
using FbRider.AllPlay.Services;

namespace FbRider.AllPlay.Tests.Services;

[TestFixture]
public class AllPlayServiceTests
{
    private Mock<IYahooFantasySportsApiClient> _apiClientMock;
    private Mock<ILeagueService> _leagueServiceMock;
    private AllPlayService _allPlayService;

    private const string AccessToken = "access_token";

    [SetUp]
    public void SetUp()
    {
        _apiClientMock = new Mock<IYahooFantasySportsApiClient>();
        _leagueServiceMock = new Mock<ILeagueService>();
        _allPlayService = new AllPlayService(_apiClientMock.Object, _leagueServiceMock.Object, new AllPlayScoreService());
    }

    [Test]
    public void GetWeeklyBeasts_LeagueStandingsNotFound_Throws()
    {
        // var leagueData = new LeagueBuilder().Build();
        // _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
        //Assert.ThrowsAsync<InvalidOperationException>(() => _allPlayService.GetWeeklyAllPlay(AccessToken, leagueData.LeagueKey, 1));
    }
}
