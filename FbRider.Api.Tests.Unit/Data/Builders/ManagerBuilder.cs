using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders;

public class ManagerBuilder
{
    private Manager _manager;

    public ManagerBuilder()
    {
        _manager = new Manager()
        {
            ManagerId = "1",
            Nickname = "default_manager",
            IsCurrentLogin = "0"
        };
    }

    public ManagerBuilder WithLoggedInUser()
    {
        _manager.IsCurrentLogin = "1";
        return this;
    }

    public ManagerBuilder WithCommissioner()
    {
        _manager.IsCommissioner = "1";
        return this;
    }

    public Manager Build()
    {
        return _manager;
    }
}