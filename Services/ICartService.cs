using System;
using PersonalProject.Models.ShoppingCartProcess;

namespace PersonalProject.Services;

public interface ICartService
{
 Task<ShoppingCart> AddItemAsync(string merchantId, string UserId, CartItem item);
 Task<ShoppingCart> GetCartAsync(string merchantId, string UserId);
Task<ShoppingCart> RemoveItemAsync(string merchantId, string UserId, string productId);
}
