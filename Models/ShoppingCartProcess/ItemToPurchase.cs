using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalProject.Models.ShoppingCartProcess;


public class ItemToPurchase
{
    [Key]
    public int Id { get; set; } // Unique identifier for the cart item (not product ID)
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; } // Saved from Product.Price
    
    // Relationship
    public int ProductId { get; set; }
    public CartItem Product { get; set; } = null!;
    
}
