namespace FbRider.Application.Services;

public interface IUserService
{
    Task<UserToken> GetUserTokenAsync(string userEmail);

    Task<UserProfile> GetUserProfileAsync(string userEmail);

    Task AddOrUpdateUserTokenAsync(UserToken userToken);

    Task<string> GetAccessTokenAsync(string userEmail);
}
