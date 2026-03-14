namespace FbRider.Application.Services
{
    public class UserService(IUserTokenRepository userTokenRepository, ISignInApiClient signInApiClient) : IUserService
    {
        public async Task<UserToken> GetUserTokenAsync(string userEmail)
        {
            return await userTokenRepository.GetUserTokenAsync(userEmail);
        }

        public async Task<UserProfile> GetUserProfileAsync(string userEmail)
        {
            string accessToken = (await userTokenRepository.GetUserTokenAsync(userEmail)).AccessToken;
            var user = await signInApiClient.GetCurrentUser(accessToken);
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
