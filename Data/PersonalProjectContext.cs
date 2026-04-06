using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Models;
using PersonalProject.Models.Restaurant;
using PersonalProject.Models.GpBootstrap;

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
        
        public DbSet<PersonalProject.Models.PortfolioItem> PortfolioItem { get; set; } = default!;
        public DbSet<PersonalProject.Models.Restaurant.RestaurantMenu> RestaurantMenu { get; set; } = default!;
        public DbSet<PersonalProject.Models.Restaurant.RestaurantEntity> RestaurantEntity { get; set; } = default!;
        public DbSet<PersonalProject.Models.GpBootstrap.GpBootstrap> GpBootstrap { get; set; } = default!;

        public DbSet<PersonalProject.Models.PaymentAggregator.Transaction> Transaction { get; set; } = default!;
        public DbSet<PersonalProject.Models.ShoppingCartProcess.CartItem> CartItem { get; set; } = default!;
        public DbSet<PersonalProject.Models.ShoppingCartProcess.Order> Order { get; set; } = default!;

        public DbSet<PersonalProject.Models.ShoppingCartProcess.OrderDetail> OrderDetails { get; set; } = default!;

        public DbSet<PersonalProject.Models.ShoppingCartProcess.ShoppingCart> Payment { get; set; } = default!;
    }
}
