using System;

namespace PersonalProject.Models.ShoppingCartProcess;


public class CartItem
{
    public int Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price  { get; set; }

    public string Description { get; set; } = string.Empty;
    
}
