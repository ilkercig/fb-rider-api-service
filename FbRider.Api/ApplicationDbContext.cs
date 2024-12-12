using FbRider.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FbRider.Api
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public required DbSet<UserToken> UserTokens { get; set; }

    }
}
