using FbRider.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FbRider.Api.Repositories
{
    public class UserTokenRepository(ApplicationDbContext dbContext) : IUserTokenRepository
    {
        public async Task<UserToken> GetUserTokenAsync(string userEmail)
        {
            var userToken = await dbContext.UserTokens.SingleOrDefaultAsync(ut => ut.Email == userEmail);
            if (userToken == null)
            {
                throw new KeyNotFoundException($"UserToken with user email '{userEmail}' was not found.");
            }
            return userToken;

        }

        public async Task AddOrUpdateUserTokenAsync(UserToken userToken)
        {
            // Check if a token already exists for the user's email
            var existingToken = await dbContext.UserTokens
                .FirstOrDefaultAsync(ut => ut.Email == userToken.Email);

            if (existingToken == null)
            {
                // Initialize created and modified dates for a new token
                userToken.CreatedDate = DateTimeOffset.UtcNow;
                userToken.ModifiedDate = DateTimeOffset.UtcNow;

                // Add the new token to the database
                await dbContext.UserTokens.AddAsync(userToken);
            }
            else
            {
                // Update the existing token's properties
                existingToken.AccessToken = userToken.AccessToken;
                existingToken.RefreshToken = userToken.RefreshToken;
                existingToken.TokenExpiration = userToken.TokenExpiration;

                // Update only the modified date for the existing token
                existingToken.ModifiedDate = DateTimeOffset.UtcNow;

                dbContext.UserTokens.Update(existingToken); // Explicitly marking as updated
            }

            // Save changes to the database
            await dbContext.SaveChangesAsync();
        }


        public async Task RemoveUserTokenAsync(string userEmail)
        {
            var userToken = await dbContext.UserTokens.FirstOrDefaultAsync(ut => ut.Email == userEmail);
            if (userToken != null)
            {
                dbContext.UserTokens.Remove(userToken);
                await dbContext.SaveChangesAsync();
            }
        }
    }

}
