using System;
using PersonalProject.Models.ShoppingCartProcess;
using PersonalProject.Data;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Controllers;


namespace PersonalProject.Services;

public class OrderService : IOrderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PersonalProjectContext _personalProjectContext;
    public OrderService(IHttpContextAccessor httpContextAccessor, PersonalProjectContext personalProjectContext)
    {
        _personalProjectContext = personalProjectContext;
        _httpContextAccessor = httpContextAccessor;
    
    }
    public async Task<Order> CreateOrderAsync(DTOCheckoutRequest request, 
        ShoppingCart shoppingCart, string merchantId, string userId)
    {
        var order = new Order
        {
            MerchantId = merchantId,
            UserId = userId,
            FullName = request.FullName,
            Email = request.Email,
            ShippingAddress = request.ShippingAddress,
            PaymentMethod = request.PaymentMethod,
            Note = request.Note,
            TotalAmount = shoppingCart.Items.Sum(i => i.PricePerUnit * i.Quantity),
            Status = "Pending",
            CreatedDate = DateTime.UtcNow
        };

        _personalProjectContext.Order.Add(order);
        await _personalProjectContext.SaveChangesAsync();

        // Add order items
        foreach (var item in shoppingCart.Items)
        {
            var oderDetail = new OrderDetail
            {
                OrderId = order.Id,
                ProductId = item.ProductId.ToString(),
                ProductName = item.Product.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.PricePerUnit
            };
            _personalProjectContext.OrderDetails.Add(oderDetail);
        }
        await _personalProjectContext.SaveChangesAsync();

        return order;
    }

/*************  ✨ Windsurf Command ⭐  *************/
        /// <summary>
        /// Get an order by order id.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>The order if found, otherwise null.</returns>
/*******  837b6275-9448-4879-bc21-4bf4d4f8ab30  *******/
    public async Task<Order?> GetOrderAsync(string orderId)
    {
        if (!int.TryParse(orderId, out int id))
        {
            return null;
        }
        return await _personalProjectContext.Order.FindAsync(id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByMerchantAsync(string merchantId)
    {
        return await _personalProjectContext.Order
            .Where(o => o.MerchantId == merchantId)
            .ToListAsync();
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _personalProjectContext.Order.FindAsync(orderId);
        if (order == null)
        {
            return false;
        }
        order.Status = status;
        await _personalProjectContext.SaveChangesAsync();
        return true;
    }
}
