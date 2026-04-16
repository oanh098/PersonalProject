using System;
using Microsoft.AspNetCore.Razor.Language;

namespace PersonalProject.Models.ShoppingCartProcess;

public class ShoppingCart
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;


    // Relationship: 1 ShoppingCart has Many itemsToPurchase
    public List<ItemToPurchase> Items { get; set; } = new List<ItemToPurchase>();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    public decimal SubTotalMoney { get; set; }
    public decimal VatAmount { get; set; } // Value Added Tax
    public decimal TotalMoney { get; set;}

    // public void PlaceOrder() { /* Logic to finalize */ }
    // public void CancelOrder() { /* Logic to clear cart */ }

}
//Shopping Cart entity serves as the "Live Work-in-Progress" area
// A. Session Persistence (The "Don't Make Me Start Over" Rule)
//If a user adds a "Nitro Cold Brew" to their cart in District 1 but their 
//phone battery dies or they close the browser, the Entity saves that data in the database.
//When they return, the system checks for an existing cart linked to their UserId and MerchantId.
//If found, it loads that cart instead of starting fresh. This ensures users don't lose their

//B. Inventory Reservation & Validation
//Before the checkout even starts, the Shopping Cart entity allows 
//the server to check: "Does the Thu Duc stall actually have 5 sugar canes 
//left for this user?" It acts as a pre-check zone to ensure the order is actually possible.

// C. Price and Discount Calculation