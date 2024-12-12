using FbRider.Api.Models;

namespace FbRider.Api.Repositories
{
    public interface IUserTokenRepository
    {
        Task<UserToken> GetUserTokenAsync(string userEmail);
        Task AddOrUpdateUserTokenAsync(UserToken userToken);
        Task RemoveUserTokenAsync(string userEmail);
    }

}
