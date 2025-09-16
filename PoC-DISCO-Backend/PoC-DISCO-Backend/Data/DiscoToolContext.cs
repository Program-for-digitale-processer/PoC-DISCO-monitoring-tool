using Microsoft.EntityFrameworkCore;
using PoC_DISCO_Backend.Models;

namespace PoC_DISCO_Backend.Data;

public class DiscoToolContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    
    public DiscoToolContext(DbContextOptions<DiscoToolContext> options) : base(options)
    {
        //
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
        });
    }
}