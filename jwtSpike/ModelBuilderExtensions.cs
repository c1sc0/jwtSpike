using jwtSpike.Models;
using Microsoft.EntityFrameworkCore;

namespace jwtSpike
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = Guid.NewGuid(),
                Name = "test",
                Password = CryptoHelper.GeneratePasswordHash("password")
            });
        }
    }
}
