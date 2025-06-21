using System;

namespace PersonalProject.Models.Restaurant;

public class RestaurantMenu
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int RestaurantId { get; set; }
    public RestaurantEntity? RestaurantEntity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? ImageUrl { get; set; } // Optional: URL for the menu item image
    public string? Category { get; set; } // Optional: Category of the menu item (e.g., appetizer, main course, dessert) 
    public bool IsAvailable { get; set; } = true; // Optional: Availability status of the menu item
    public string? SpecialInstructions { get; set; } // Optional: Special instructions for the menu item (e.g., gluten-free, vegetarian)
    public string? Ingredients { get; set; } // Optional: List of ingredients used in the menu item
    public string? NutritionalInfo { get; set; } // Optional: Nutritional information for the menu item  

    public string? PreparationTime { get; set; } // Optional: Estimated preparation time for the menu item
    public string? ServingSize { get; set; } // Optional: Serving size for the menu item
    public string? SpicinessLevel { get; set; } // Optional: Spiciness level for the menu item (e.g., mild, medium, hot)
    public string? DietaryRestrictions { get; set; } // Optional: Dietary restrictions (e.g., vegan, nut-free)
    public string? CuisineType { get; set; } // Optional: Type of cuisine (e.g., Italian, Chinese, Mexican)
    public string? ChefRecommendation { get; set; } // Optional: Chef's recommendation for the menu item
    public string? Allergens { get; set; } // Optional: Common allergens present in the menu item (e.g., dairy, nuts, gluten)
    public string? CookingMethod { get; set; } // Optional: Cooking method used for the menu item (e.g., grilled, fried, baked)  
    public string? PairingSuggestions { get; set; } // Optional: Suggested pairings (e.g., wine, sides) for the menu item
    public string? CustomerReviews { get; set; } // Optional: Customer reviews or ratings for the menu item
    public string? SeasonalAvailability { get; set; } // Optional: Seasonal availability of the menu item (e.g., summer, winter)
    public string? PortionSize { get; set; } // Optional: Portion size for the menu item (e.g., small, medium, large)
    public bool IsSpecial { get; set; } // Indicates if the menu item is a special
    
}
