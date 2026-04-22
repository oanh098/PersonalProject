using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PersonalProject.Models.ShoppingCartProcess;
using System.Text.Json.Serialization;

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

    public async Task<ShoppingCart> AddItemAsync(string merchantId, 
    string userId, ItemToPurchase item)
    {
        string cacheKey = $"cart:{merchantId}:{userId}";
        //var shoppingCart = await GetCartAsync(merchantId, UserId);

        // 1. MUST GET existing cart first
            var shoppingCart = await GetCartAsync(merchantId, userId) ?? new ShoppingCart()
            {
                UserId = userId,
                MerchantId = merchantId,
                Items = new List<ItemToPurchase>()
            };

            // 2. Check if product already exists to avoid duplicate
            var existingItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                    
                }
                else
                {
                    shoppingCart.Items.Add(item);
                }

        

        // 3. RECALCULATE all totals so they aren't 0
            UpdateCartTotals(shoppingCart);
           

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) // Cache for 7 days
            };
        
        // 4. Save the updated list back to Redis
        await SaveCartAsync(merchantId, userId, shoppingCart);
        // await _cache.SetStringAsync(cacheKey, 
        // System.Text.Json.JsonSerializer.Serialize(shoppingCart), options);
        // Log the Key
        Console.WriteLine($"--- CACHE DEBUG ---");
        Console.WriteLine($"Cache Key: {cacheKey}");

        // Log the Object (Serialized so you can see the Items inside)
        string debugJson = System.Text.Json.JsonSerializer.Serialize(shoppingCart, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine($"Shopping Cart Content: {debugJson}");
        Console.WriteLine($"-------------------");
        return shoppingCart;
    }

    public async Task<ShoppingCart> GetCartAsync(string merchantId, string UserId)
    {
        var cached = _cache.GetStringAsync($"cart:{merchantId}:{UserId}");
        if (string.IsNullOrEmpty(await cached))
        {
            return new ShoppingCart();
        }
        return JsonSerializer.Deserialize<ShoppingCart>(await cached) 
        ?? new ShoppingCart();
        
    }

    // Helper: Keeps math consistent everywhere
    private void UpdateCartTotals(ShoppingCart cart)
    {
        cart.SubTotalMoney = cart.Items.Sum(i => i.Quantity * i.PricePerUnit);
        cart.VatAmount = cart.SubTotalMoney * 0.08m;
        cart.TotalMoney = cart.SubTotalMoney + cart.VatAmount;
        cart.LastUpdated = DateTime.UtcNow;

        
    }

    public async Task<ShoppingCart> RemoveItemAsync(string merchantId, 
    string UserId, int productId)
    {
        string cacheKey = $"cart:{merchantId}:{UserId}";
        // 1. Get the cart
        var shoppingCart = await GetCartAsync(merchantId, UserId);
        // 2. Safety Check: If no cart exists, just return null or a new empty cart
        if (shoppingCart == null || shoppingCart.Items == null)
        {
            return new ShoppingCart(); 
        }
        // 3. Remove based on ProductId (since item.Id was 0 in your debug data)
        shoppingCart.Items.RemoveAll(i => i.ProductId == productId);

        // Update cache after removal 
        // 4. Recalculate the Math (Otherwise the total stays the same!)

        UpdateCartTotals(shoppingCart);
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) // Cache for 7 days
        };
        // 5. Save using your SaveCartAsync method to keep code DRY (Don't Repeat Yourself)
        await SaveCartAsync(merchantId, UserId, shoppingCart);
        // await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(shoppingCart), options);
        return shoppingCart;
    }

    public async Task SaveCartAsync(string merchantId, string userId, ShoppingCart cart)
    {
        // 1. Define the unique key for this specific user at this specific stall
        string cacheKey = $"cart:{merchantId}:{userId}";

        // 2. Configure JSON options to handle complex objects
        var options = new JsonSerializerOptions
        {
            // Prevents "Circular Reference" errors if your Product object links back to Category, etc.
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            // Makes the JSON easier to read when you are debugging in the console
            WriteIndented = true 
        };

        // 3. Convert your C# ShoppingCart object into a JSON string
        string jsonData = JsonSerializer.Serialize(cart, options);

        // 4. Set the "Time to Live" (TTL)
        // For a retail shop, 1 to 7 days is usually standard
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) 
        };

        // 5. Save the string to Redis
        await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);
    }
    
    

}
