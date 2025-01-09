using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders
{
    internal class UserBuilder
    {
        private User _user;

        public UserBuilder()
        {
            _user = new User()
            {
                Guid = Guid.NewGuid().ToString(),
                Games = [new GameBuilder().Build(), new GameBuilder().WithGameKey("a_game").Build()]
            };
        }

        public User Build()
        {
            return _user;
        }
    }
}
