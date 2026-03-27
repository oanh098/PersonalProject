using System;
using PersonalProject.Controllers;
using PersonalProject.Models.ShoppingCartProcess;

namespace PersonalProject.Services;

public interface IOrderService
{
    // 1. The "Core" Logic: Create the order tied to a Merchant
    Task<Order> CreateOrderAsync(DTOCheckoutRequest request, 
    ShoppingCart shoppingCart,string merchantId, string userId);
    
    // 2. Retrieval: Get a single order's details
    Task<Order?> GetOrderAsync(string orderId);

    // 3. History: Get all in shopping cart for a specific stall (e.g., District 1)
    Task<IEnumerable<Order>> GetOrdersByMerchantAsync(string merchantId);

    // 4. Management: Update status (Pending -> Processing -> Completed)
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
}
 