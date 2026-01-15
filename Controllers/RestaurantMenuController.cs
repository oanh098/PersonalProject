using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Data;
using PersonalProject.Models.Restaurant;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace PersonalProject.Controllers
{
    public class RestaurantMenuController : Controller
    {
        private readonly PersonalProjectContext _context;
        private readonly Cloudinary _cloudinary;

        public RestaurantMenuController(PersonalProjectContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: RestaurantMenu
        public async Task<IActionResult> Index()
        {
            return View(await _context.RestaurantMenu.ToListAsync());
        }

        // GET: RestaurantMenu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantMenu = await _context.RestaurantMenu
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantMenu == null)
            {
                return NotFound();
            }

            return View(restaurantMenu);
        }

        // GET: RestaurantMenu/Create
        public IActionResult Create()
        {
            var random = new Random();

            // Sample lorem data arrays
            string[] dishNames = { "Lorem Steak", "Ipsum Pasta", "Dolor Pizza", "Sit Salad", "Amet Soup" };
            string[] descriptions = {
                "A delicious blend of flavors and textures.",
                "Classic recipe with a modern twist.",
                "Perfect for any occasion.",
                "A favorite among our guests.",
                "Prepared fresh daily with local ingredients."
            };
            string[] categories = { "Starter", "Main Course", "Dessert", "Beverage" };
            string[] ingredients = { "Tomato, Basil, Cheese", "Chicken, Rice, Spices", "Beef, Onion, Garlic", "Fish, Lemon, Dill" };

            var menu = new RestaurantMenu
            {
                Name = dishNames[random.Next(dishNames.Length)],
                Description = descriptions[random.Next(descriptions.Length)],
                Price = Math.Round((decimal)(random.NextDouble() * 50 + 10), 2),
                RestaurantId = random.Next(1, 10), // Adjust as needed
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RestaurantEntity = new RestaurantEntity { Id = random.Next(1, 10) },
                Category = categories[random.Next(categories.Length)],
                IsAvailable = random.Next(0, 2) == 1,
                SpecialInstructions = "No special instructions",
                Ingredients = ingredients[random.Next(ingredients.Length)],
                NutritionalInfo = "Calories: 500, Fat: 10g, Carbs: 20g, Protein: 30g",
                PreparationTime = random.Next(10, 60) + " minutes",
                ServingSize = random.Next(1, 5) + " servings",
                SpicinessLevel = random.Next(0, 3) == 0 ? "Mild" : random.Next(0, 2) == 0 ? "Medium" : "Hot",
                DietaryRestrictions = "Gluten-free",
                CuisineType = "Italian", // Example cuisine type
                ChefRecommendation = "Highly recommended by our chef!",
                Allergens = "Contains nuts",
                CookingMethod = "Grilled", // Example cooking method
                PairingSuggestions = "Pairs well with red wine",
                CustomerReviews = "4.5 stars based on 100 reviews",
                SeasonalAvailability = "Available year-round",
                PortionSize = "Large", // Example portion size
                IsSpecial = random.Next(0, 2) == 1 // Randomly set if it's a special item
                
                // Add more fields as needed
            };
            
           

            return View(menu);
        }

        // POST: RestaurantMenu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Description,Price,RestaurantId,Category,IsAvailable,SpecialInstructions,Ingredients,NutritionalInfo,PreparationTime,ServingSize,SpicinessLevel,DietaryRestrictions,CuisineType,ChefRecommendation,Allergens,CookingMethod,PairingSuggestions,CustomerReviews,SeasonalAvailability,PortionSize,IsSpecial")]
            RestaurantMenu restaurantMenu, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if(ImageFile != null && ImageFile.Length > 0)
                {
                    using (var stream = ImageFile.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(ImageFile.FileName, stream),
                            Folder = "RestaurantMenu" // <-- folder specified here
                        };
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        restaurantMenu.ImageUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                

                _context.Add(restaurantMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                // Log or inspect errors here
            }
            return View(restaurantMenu);
        }

        // GET: RestaurantMenu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantMenu = await _context.RestaurantMenu.FindAsync(id);
            if (restaurantMenu == null)
            {
                return NotFound();
            }
            return View(restaurantMenu);
        }

        // POST: RestaurantMenu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
        [Bind("Id,Name,Description,Price,RestaurantId,CreatedAt,UpdatedAt,ImageUrl,Category,IsAvailable,SpecialInstructions,Ingredients,NutritionalInfo,PreparationTime,ServingSize,SpicinessLevel,DietaryRestrictions,CuisineType,ChefRecommendation,Allergens,CookingMethod,PairingSuggestions,CustomerReviews,SeasonalAvailability,PortionSize,IsSpecial")]
        RestaurantMenu restaurantMenu, IFormFile? ImageFile)
        {
            if (id != restaurantMenu.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        using (var stream = ImageFile.OpenReadStream())
                        {
                            var uploadParams = new ImageUploadParams()
                            {
                                File = new FileDescription(ImageFile.FileName, stream),
                                Folder = "RestaurantMenu"
                            };
                            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                            restaurantMenu.ImageUrl = uploadResult.SecureUrl.ToString();
                        }
                    }

                    // Ensure DateTimeKind.Utc for PostgreSQL compatibility
                    restaurantMenu.CreatedAt = DateTime.SpecifyKind(restaurantMenu.CreatedAt, DateTimeKind.Utc);
                    restaurantMenu.UpdatedAt = DateTime.UtcNow;

                    _context.Update(restaurantMenu);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantMenuExists(restaurantMenu.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(restaurantMenu);
        }


        // GET: RestaurantMenu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantMenu = await _context.RestaurantMenu
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantMenu == null)
            {
                return NotFound();
            }

            return View(restaurantMenu);
        }

        // POST: RestaurantMenu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurantMenu = await _context.RestaurantMenu.FindAsync(id);
            if (restaurantMenu != null)
            {
                _context.RestaurantMenu.Remove(restaurantMenu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantMenuExists(int id)
        {
            return _context.RestaurantMenu.Any(e => e.Id == id);
        }
    }
}
