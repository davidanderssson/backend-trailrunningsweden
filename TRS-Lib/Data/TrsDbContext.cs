using Microsoft.EntityFrameworkCore;
using TRS_Lib.Models;

namespace TRS_Lib.Data;

public class TrsDbContext(DbContextOptions<TrsDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<OneTimePassword> OneTimePasswords => Set<OneTimePassword>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(u => u.Id);
            user.Property(u => u.FirstName).HasMaxLength(100);
            user.Property(u => u.LastName).HasMaxLength(100);
            user.Property(u => u.Email).HasMaxLength(320);
            user.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<OneTimePassword>(otp =>
        {
            otp.HasKey(o => o.Id);
            otp.Property(o => o.CodeHash).HasMaxLength(100);
            otp.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            otp.HasIndex(o => o.UserId);
        });

        modelBuilder.Entity<RefreshToken>(token =>
        {
            token.HasKey(r => r.Id);
            token.Property(r => r.TokenHash).HasMaxLength(64);
            token.HasIndex(r => r.TokenHash).IsUnique();
            token.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            token.HasIndex(r => r.UserId);
        });

        modelBuilder.Entity<Organization>(organization =>
        {
            organization.HasKey(o => o.Id);
            organization.Property(o => o.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<OrganizationMember>(member =>
        {
            member.HasKey(m => new { m.OrganizationId, m.UserId });
            member.HasOne(m => m.Organization)
                .WithMany()
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
            member.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            member.HasIndex(m => m.UserId);
            // Stored as text ("Requested", "Owner", ...) so the data is readable and enum reordering can't corrupt it
            member.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
            member.Property(m => m.MemberType).HasConversion<string>().HasMaxLength(20);
        });
    }
}
