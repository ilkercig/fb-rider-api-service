using FbRider.Api.Models;
using MongoDB.Driver;

namespace FbRider.Api.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly IMongoCollection<UserToken> _userTokens;

        public UserTokenRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoConnection");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("FbRiderDb");
            _userTokens = database.GetCollection<UserToken>("UserTokens");
        }

        public async Task<UserToken> GetUserTokenAsync(string userEmail)
        {
            var userToken = await _userTokens.Find(ut => ut.Email == userEmail).FirstOrDefaultAsync();
            if (userToken == null)
            {
                throw new KeyNotFoundException($"UserToken with user email '{userEmail}' was not found.");
            }
            return userToken;
        }

        public async Task AddOrUpdateUserTokenAsync(UserToken userToken)
        {
            var filter = Builders<UserToken>.Filter.Eq(ut => ut.Email, userToken.Email);
            var existingToken = await _userTokens.Find(filter).FirstOrDefaultAsync();

            if (existingToken == null)
            {
                userToken.CreatedDate = DateTimeOffset.UtcNow;
                userToken.ModifiedDate = DateTimeOffset.UtcNow;
                await _userTokens.InsertOneAsync(userToken);
            }
            else
            {
                userToken.Id = existingToken.Id; // Keep the same ID
                userToken.CreatedDate = existingToken.CreatedDate;
                userToken.ModifiedDate = DateTimeOffset.UtcNow;
                await _userTokens.ReplaceOneAsync(filter, userToken);
            }
        }

        public async Task RemoveUserTokenAsync(string userEmail)
        {
            await _userTokens.DeleteOneAsync(ut => ut.Email == userEmail);
        }
    }
}
