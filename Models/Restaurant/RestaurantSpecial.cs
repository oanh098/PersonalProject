using System;

namespace PersonalProject.Models.Restaurant;

public class RestaurantSpecial
{
    public int Id { get; set; }
    public string? Special { get; set; }
    public string? Description { get; set; }

    public int? RestaurantId { get; set; }

    public string? ImageUrl { get; set; }

    public string SpecialType { get; set; } // e.g., "Daily Special", "Seasonal Special", "Holiday Special"

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
