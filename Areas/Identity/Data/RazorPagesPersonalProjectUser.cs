using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PersonalProject.Areas.Identity.Data;

// Add profile data for application users by adding properties to the RazorPagesPersonalProjectUser class
public class RazorPagesPersonalProjectUser : IdentityUser
{
    // configures unidirectional navigation property for all relationships on User

    public virtual ICollection<RazorPagesPersonalProjectUserRole> UserRoles { get; set; } = new List<RazorPagesPersonalProjectUserRole>();
    
    

    // Add additional properties here
    // For example, you can add a property for the user's full name
    [Required]
    [MaxLength(100)]
    [PersonalData]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    [PersonalData]
    public string LastName { get; set; } = string.Empty;

    // You can also add properties for profile picture URL, bio, etc.
    [PersonalData]
    public string? ProfilePictureUrl { get; set; }
    [PersonalData]
    public DateTime DOB { get; set; } 


    // Add any other custom properties you need for your application
}

