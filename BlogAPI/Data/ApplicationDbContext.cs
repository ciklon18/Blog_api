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
    public DbSet<Community> Communities { get; set; } = null!;
    public DbSet<UserCommunityRole> UserCommunityRoles { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    
    public DbSet<Tag> Tags { get; set; } = null!;
    
    public DbSet<PostTag> PostTags { get; set; } = null!;
    
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCommunityRole>()
            .HasKey(ucr => new { ucr.UserId, ucr.CommunityId });
        modelBuilder.Entity<PostTag>()
            .HasKey(pt => new { pt.PostId, pt.TagId });

        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId);
        
        base.OnModelCreating(modelBuilder);

    }
}