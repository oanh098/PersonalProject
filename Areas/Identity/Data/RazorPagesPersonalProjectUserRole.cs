using System;
using Microsoft.AspNetCore.Identity;

namespace PersonalProject.Areas.Identity.Data;

// This is the custom join entity for the many-to-many relationship between users and roles
// It inherits from IdentityUserRole<TKey>
public class RazorPagesPersonalProjectUserRole : IdentityUserRole<string>
{
    // Navigation properties to the user and role entities
    // These are important for Entity Framework to understand the relationships
    public virtual RazorPagesPersonalProjectUser User { get; set; } = null!; // Use null-forgiving operator to indicate that this will be set by EF
    public virtual RazorPagesPersonalProjectRole Role { get; set; } = null!; // Use null-forgiving operator to indicate that this will be set by EF

}
