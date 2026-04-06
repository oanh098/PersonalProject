using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PersonalProject.Models.ShoppingCartProcess;

namespace PersonalProject.Services;

public class CartService : ICartService
{
    private readonly IDistributedCache _cache;
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CartService(IHttpContextAccessor httpContextAccessor, 
    IDistributedCache cache)
     
    {
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    public async Task<ShoppingCart> AddItemAsync(string merchantId, string UserId, CartItem item)
    {
        string cacheKey = $"cart:{merchantId}:{UserId}";
        var cart = await GetCartAsync(merchantId, UserId);
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
            existingItem.Price = item.Price; // Update price in case it has changed
        }
        else
        {
            cart.Items.Add(item);
        }
            var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) // Cache for 7 days
        };
        await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(cart), options);

        return cart;
    }

    public async Task<ShoppingCart> GetCartAsync(string merchantId, string UserId)
    {
        var cached = _cache.GetStringAsync($"cart:{merchantId}:{UserId}");
        if (string.IsNullOrEmpty(await cached))
        {
            return new ShoppingCart();
        }
        return JsonSerializer.Deserialize<ShoppingCart>(await cached) ?? new ShoppingCart();
        
    }

    public async Task<ShoppingCart> RemoveItemAsync(string merchantId, string UserId, string productId)
    {
        string cacheKey = $"cart:{merchantId}:{UserId}";
        var cart = await GetCartAsync(merchantId, UserId);
        cart.Items.RemoveAll(i => i.ProductId == productId);

        // Update cache after removal 
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) // Cache for 7 days
        };
        await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(cart), options);
        return cart;
    }
    
    

}
