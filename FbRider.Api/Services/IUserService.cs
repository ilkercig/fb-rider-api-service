using FbRider.Api.DTOs;
using FbRider.Api.Models;

namespace FbRider.Api.Services;

public interface IUserService
{
    Task<UserToken> GetUserTokenAsync(string userEmail);

    Task<YahooUser> GetYahooUserAsync(string userEmail);

    Task AddOrUpdateUserTokenAsync(UserToken userToken);

    Task<string> GetAccessTokenAsync(string userEmail);
}