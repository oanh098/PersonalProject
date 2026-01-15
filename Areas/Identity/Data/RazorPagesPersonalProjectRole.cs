using System;
using Microsoft.AspNetCore.Identity;

namespace PersonalProject.Areas.Identity.Data;


public class RazorPagesPersonalProjectRole : IdentityRole
{
    // Navigation properties to your custom UserRole join entity
    // This connects the role to users who has the role through custom join table
    public virtual ICollection<RazorPagesPersonalProjectUserRole> UserRoles { get; set; } = new List<RazorPagesPersonalProjectUserRole>();

    // Add any additional properties you need for your roles
    // For example, you can add a description or permissions associated with the role
}
