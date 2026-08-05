using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NoatunCrewing.Data;

public class NoatunCrewingContext(DbContextOptions<NoatunCrewingContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Identity table names are the EF Core defaults (AspNetUsers, AspNetRoles, ...).
        // Add crew entity configuration via builder.ApplyConfigurationsFromAssembly(...) once entities exist.

        builder.Entity<ApplicationUserGroup>()
            .HasKey(ug => new { ug.ApplicationUserId, ug.GroupId });

        builder.Entity<ApplicationUserGroup>()
            .HasOne(ug => ug.ApplicationUser)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ug => ug.ApplicationUserId);

        builder.Entity<ApplicationUserGroup>()
            .HasOne(ug => ug.Group)
            .WithMany(g => g.UserGroups)
            .HasForeignKey(ug => ug.GroupId);
    }
}