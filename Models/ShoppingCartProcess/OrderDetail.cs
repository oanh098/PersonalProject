using System;

namespace PersonalProject.Models.ShoppingCartProcess;

//OrderDetail is for the Order History phase.
public class OrderDetail
{
    public int Id { get; set; }
    
    // Link back to the parent Order
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty; // Store name in case you rename the product later
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // The price they paid at that moment
}
