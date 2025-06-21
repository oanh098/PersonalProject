using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Models;
using PersonalProject.Models.Restaurant;

namespace PersonalProject.Data
{
    public class PersonalProjectContext : DbContext
    {
        public PersonalProjectContext(DbContextOptions<PersonalProjectContext> options)
            : base(options)
        {
        }

        public DbSet<PersonalProject.Models.Movie> Movie { get; set; } = default!;
        public DbSet<PersonalProject.Models.Skill> Skill { get; set; } = default!;
        
        public DbSet<PersonalProject.Models.Portfolio> Portfolio { get; set; } = default!;
        public DbSet<PersonalProject.Models.Restaurant.RestaurantMenu> RestaurantMenu { get; set; } = default!;
        public DbSet<PersonalProject.Models.Restaurant.RestaurantEntity> RestaurantEntity { get; set; } = default!;
    }
}
