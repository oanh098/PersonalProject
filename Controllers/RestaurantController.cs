using System;
using Microsoft.AspNetCore.Mvc;
using PersonalProject.Data;
using PersonalProject.Models.Restaurant;

namespace PersonalProject.Controllers;

public class RestaurantController : Controller
{
    private readonly PersonalProjectContext _context;

    public RestaurantController(PersonalProjectContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var restaurantEntities = _context.RestaurantEntity.ToList();
        var restaurantMenus = _context.RestaurantMenu.ToList();
        var viewModels = new RestaurantIndexViewModels
        {
            RestaurantEntity = restaurantEntities,
            RestaurantMenu = restaurantMenus
        };
        return View(viewModels);

       
    }

   
    
}
