using System;

namespace PersonalProject.Models.Restaurant;

public class RestaurantIndexViewModels
{
    public List<RestaurantEntity> RestaurantEntity { get; set; } = new List<RestaurantEntity>();
    public List<RestaurantMenu> RestaurantMenu { get; set; } = new List<RestaurantMenu>();

}
