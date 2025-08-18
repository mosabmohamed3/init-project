using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using REM.Application.Common.Interfaces;
using REM.Domain.Entities;
using REM.Domain.Entities.Identity;

namespace REM.Infrastructure.Context;

public class AppDbContext
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >
{
    private readonly ICurrentUser _currentUser;

    public AppDbContext(DbContextOptions options)
        : base(options)
    {
        _currentUser = this.GetService<ICurrentUser>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        ConfigureIdentity(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.Modified_At = DateTime.UtcNow;
                    entry.Entity.Modified_By =
                        (
                            entry.Entity.Modified_By is not null
                            && entry.Entity.Modified_By != Guid.Empty
                        )
                            ? entry.Entity.Modified_By
                            : _currentUser.GetUserId();
                    break;
                case EntityState.Added:
                    entry.Entity.Created_At = DateTime.UtcNow;
                    entry.Entity.Created_By =
                        entry.Entity.Created_By != Guid.Empty
                            ? entry.Entity.Created_By
                            : _currentUser.GetUserId();
                    break;
                default:
                    break;
            }
        }

        var IdentityEntries = ChangeTracker.Entries<ApplicationUser>();

        foreach (var entry in IdentityEntries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.Modified_At = DateTime.UtcNow;
                    entry.Entity.Modified_By = _currentUser.GetUserId();
                    break;
                case EntityState.Added:
                    entry.Entity.Created_At = DateTime.UtcNow;
                    entry.Entity.Created_By = _currentUser.GetUserId();
                    break;
                default:
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Ignore<IdentityUserClaim<Guid>>()
            .Ignore<IdentityUserLogin<Guid>>()
            .Ignore<IdentityRoleClaim<Guid>>()
            .Ignore<IdentityUserToken<Guid>>();

        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.HasMany(a => a.UserRoles).WithOne(a => a.User).HasForeignKey(a => a.UserId);
            builder.HasIndex(a => a.PhoneNumber);
            builder.ToTable("ApplicationUsers");
        });

        modelBuilder.Entity<ApplicationUserRole>(builder =>
        {
            builder.HasKey(builder => new { builder.UserId, builder.RoleId });
            builder
                .HasOne(builder => builder.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(builder => builder.UserId)
                .IsRequired();
            builder
                .HasOne(builder => builder.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(builder => builder.RoleId)
                .IsRequired();

            builder.ToTable("ApplicationUserRoles");
        });

        modelBuilder.Entity<ApplicationRole>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.HasMany(a => a.UserRoles).WithOne(a => a.Role).HasForeignKey(a => a.RoleId);

            builder.ToTable("ApplicationRoles");
        });
    }
}
