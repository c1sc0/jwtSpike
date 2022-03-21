using jwtSpike.Models;
using Microsoft.EntityFrameworkCore;

namespace jwtSpike;

public class JwtDbContext : DbContext
{
    public JwtDbContext(DbContextOptions<JwtDbContext> options) : base(options)
    {
    }

    public DbSet<User>? Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}