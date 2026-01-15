using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Areas.Identity.Data;

namespace PersonalProject.Areas.Identity.Data;

public class RazorPagesPersonalProjectAuth
    : IdentityDbContext<
        RazorPagesPersonalProjectUser,
        RazorPagesPersonalProjectRole,
        string,
        IdentityUserClaim<string>,
        RazorPagesPersonalProjectUserRole,
        IdentityUserLogin<string>,    
        IdentityRoleClaim<string>,     
        IdentityUserToken<string>
    >
{
    public RazorPagesPersonalProjectAuth(DbContextOptions<RazorPagesPersonalProjectAuth> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<RazorPagesPersonalProjectUser>(b =>
        {
            b.HasMany(e => e.UserRoles)// Navigation property on User b to the join entity e
                 .WithOne(e => e.User)// Navigation property on the join entity e to the User
                 .HasForeignKey(ur => ur.UserId)// Foreign key in the join entity that points to the User 
                 .IsRequired();// UserId is required in UserRoles
        });

        builder.Entity<RazorPagesPersonalProjectRole>(b =>
         {
             b.HasMany(e => e.UserRoles)// Navigation property on Role b to the join entity e
                  .WithOne(e => e.Role)// Navigation property on the join entity e to the Role
                  .HasForeignKey(ur => ur.RoleId)// Foreign key in the join entity that points to the Role
                  .IsRequired();// RoleId is required in UserRoles
         });

        // Configure the composite key for the join entity
        builder.Entity<RazorPagesPersonalProjectUserRole>(b =>
        {
            b.HasKey(ur => new { ur.UserId, ur.RoleId }); // Composite key for the join entity
            // You can also configure any custom properties on RazorPagesPersonalProjectUserRole here
            // For example, if you added a 'AssignedDate' property:
            // b.Property(ur => ur.AssignedDate).IsRequired();
        });
        
        // Configure other properties and relationships as needed
    }
}
