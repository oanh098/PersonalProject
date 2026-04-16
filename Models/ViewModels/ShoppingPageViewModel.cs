using System;

namespace PersonalProject.Models.ShoppingCartProcess;

public class ShoppingPageViewModel
{
    public IEnumerable<CartItem> CartItemAsProduct { get; set; } = new List<CartItem>();
    public IEnumerable<ItemToPurchase> ItemToPurchase { get; set; } = new List<ItemToPurchase>();

}
