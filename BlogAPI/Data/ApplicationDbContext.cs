using BlogAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<HierarchyAddress> HierarchyAddresses { get; set; } = null!;
    public DbSet<HousesAddress> HousesAddresses { get; set; } = null!;
    
    public DbSet<Tag> Tags { get; set; } = null!;
    
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}