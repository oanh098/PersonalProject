using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PersonalProject.Data;
using PersonalProject.Models.Restaurant;

namespace PersonalProject.Controllers;

public class RestaurantController : Controller
{
    private readonly PersonalProjectContext _context;
    private readonly IDistributedCache _cacheRestaurant;
    public RestaurantController(PersonalProjectContext context
    , IDistributedCache cacheRestaurant)
    {
        _context = context;
        _cacheRestaurant = cacheRestaurant;
    }

    public async Task<IActionResult> Index()
    {
        string cacheKey = "RestaurantIndexData";
        string? cacheData = await _cacheRestaurant.GetStringAsync(cacheKey);
        if (string.IsNullOrEmpty(cacheData))
        {
            cacheData = "This is Restaurant cached data from the database at" + DateTime.Now;
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            await _cacheRestaurant.SetStringAsync(cacheKey, cacheData, options);
        }
        var restaurantEntities = _context.RestaurantEntity.ToList();
        var restaurantMenus = _context.RestaurantMenu.ToList();
        var viewModels = new RestaurantIndexViewModels
        {
            RestaurantEntity = restaurantEntities,
            RestaurantMenu = restaurantMenus,
            CacheData = cacheData
        };
        return View(viewModels);

       
    }

   
    
}
