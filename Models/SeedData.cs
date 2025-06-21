using Microsoft.EntityFrameworkCore;
using PersonalProject.Data;

namespace PersonalProject.Models;

public class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new PersonalProjectContext(
            serviceProvider.GetRequiredService<DbContextOptions<PersonalProjectContext>>()))
        {
            // Look for any movies.
            if (context.Movie.Any())
            {
                return;   // DB has been seeded
            }
            context.Movie.AddRange(
                new Movie
                {
                    Title = "When Harry Met Sally",
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(1959, 7, 16), DateTimeKind.Utc),
                    Genre = "Comedy",
                    Price = 7.99M,
                    Rating = "PG-13"
                },
                new Movie
                {
                    Title = "Ghostbusters",
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(1984, 7, 31), DateTimeKind.Utc),
                    Genre = "Comedy",
                    Price = 8.99M,
                    Rating = "PG-13"    
                },
                new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(1959, 7, 16), DateTimeKind.Utc),
                    Genre = "Western",
                    Price = 3.99M,
                    Rating = "PG-13"            
                },
                new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(1959, 7, 16), DateTimeKind.Utc),                
                    Genre = "Western",
                    Price = 3.99M,
                    Rating = "PG-13"            
                },
                new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(1959, 7, 16), DateTimeKind.Utc),
                    Genre = "Western",
                    Price = 3.99M,
                    Rating = "PG-13"            
                }
            );
            context.SaveChanges();
        }
    }
}
