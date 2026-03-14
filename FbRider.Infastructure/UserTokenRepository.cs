using FbRider.Application;
using FbRider.Infastructure.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FbRider.Infastructure
{
    public class UserTokenRepository(IConfiguration configuration) : IUserTokenRepository
    {
        private readonly IMongoCollection<UserTokenEntity> _userTokens = Init(configuration);

        private static IMongoCollection<UserTokenEntity> Init(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoConnection");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("FbRiderDb");
            return database.GetCollection<UserTokenEntity>("UserTokens");
        }

        public async Task<UserToken> GetUserTokenAsync(string userEmail)
        {
            var userToken = await _userTokens.Find(ut => ut.Email == userEmail).FirstOrDefaultAsync();
            if (userToken == null)
            {
                throw new KeyNotFoundException($"UserToken with user email '{userEmail}' was not found.");
            }
            return new UserToken(userToken.Email, userToken.AccessToken, userToken.RefreshToken, userToken.TokenExpiration);
        }

        public async Task AddOrUpdateUserTokenAsync(UserToken userToken)
        {
            var filter = Builders<UserTokenEntity>.Filter.Eq(ut => ut.Email, userToken.Email);
            var existingToken = await _userTokens.Find(filter).FirstOrDefaultAsync();

            if (existingToken == null)
            {
                var userTokenEntity = new UserTokenEntity
                {
                    Email = userToken.Email,
                    AccessToken = userToken.AccessToken,
                    RefreshToken = userToken.RefreshToken,
                    TokenExpiration = userToken.TokenExpiration,
                    CreatedDate = DateTimeOffset.UtcNow,
                    ModifiedDate = DateTimeOffset.UtcNow
                };
                await _userTokens.InsertOneAsync(userTokenEntity);
            }
            else
            {
                existingToken.AccessToken = userToken.AccessToken;
                existingToken.RefreshToken = userToken.RefreshToken;
                existingToken.TokenExpiration = userToken.TokenExpiration;
                existingToken.ModifiedDate = DateTimeOffset.UtcNow;
                await _userTokens.ReplaceOneAsync(filter, existingToken);
            }
        }

        public async Task RemoveUserTokenAsync(string userEmail)
        {
            await _userTokens.DeleteOneAsync(ut => ut.Email == userEmail);
        }
    }
}
