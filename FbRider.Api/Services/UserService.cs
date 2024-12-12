using FbRider.Api.DTOs;
using FbRider.Api.Models;
using FbRider.Api.Repositories;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Services
{
    public class UserService(IUserTokenRepository userTokenRepository, IYahooSignInApiClient yahooSignInApiClient) : IUserService
    {
        public async Task<UserToken> GetUserTokenAsync(string userEmail)
        {
            return await userTokenRepository.GetUserTokenAsync(userEmail);
        }

        public async Task<YahooUser> GetYahooUserAsync(string userEmail)
        {
            string accessToken = (await userTokenRepository.GetUserTokenAsync(userEmail)).AccessToken;
            var user = await yahooSignInApiClient.GetCurrentUser(accessToken);
            return user;
        }

        public async Task AddOrUpdateUserTokenAsync(UserToken userToken)
        {
            await userTokenRepository.AddOrUpdateUserTokenAsync(userToken);
        }

        public async Task<string> GetAccessTokenAsync(string userEmail)
        {
            return (await userTokenRepository.GetUserTokenAsync(userEmail)).AccessToken;
        }
    }
}
