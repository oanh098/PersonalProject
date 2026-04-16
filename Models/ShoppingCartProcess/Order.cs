using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalProject.Models.ShoppingCartProcess;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } // The unique Order ID

    // --- Link to User and Merchant ---
    public string UserId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;

    // --- Snapshot of Customer Info (from DTO) ---
    public string FullName { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Note { get; set; }

    // --- Financials ---
    public decimal TotalAmount { get; set; }
    public string? DiscountCode { get; set; }
    public string PaymentMethod { get; set; } = "COD";

    // --- Tracking ---
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending, Processing, Shipping, Completed
    
    // --- Navigation Properties ---
    // OrderDetail is for the Order History phase.
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}