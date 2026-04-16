using System;
using System.ComponentModel.DataAnnotations;
namespace PersonalProject.Models.PaymentAggregator;

public class Transaction
{
    [Key]
    public Guid TransactionId { get; set; } = Guid.NewGuid();

    // The Merchant using the payment aggregator
    [Required]
    public string? MerchantId { get; set; }

    // Financial details
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be a 3-letter ISO code.")]// ISO Currency Code ("usd", "vnd", etc.)
    public string Currency { get; set; } = "USD";

    // Payment method details
    public string? PaymentMethodType { get; set; } // CreditCard, EWallet, BankTransfer
    public string? ProviderTransactionId { get; set; } // The ID return by Gateway (Stripe/ Bank) 

    // Status Management
    public TransactionStatus? Status { get; set; } = TransactionStatus.Pending;

    //Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Metadata for the Merchant's reference
    public string OrderReference { get; set; } = string.Empty; // Reference to the order being paid for

    public string? CustomerEmail { get; set; } // Optional: for sending receipts or notificationsc
}
public enum TransactionStatus
{
    Pending,// Initial state when customer clicks "Pay"
    Processing,// Sent to the bank/gateway
    Success, // Payment confirmed
    Failed,// Payment rejected
    Refunded // Money returned to customer
}
//When the payment starts, you can cache this object in Redis 
//using the TransactionId as the key: 
//await _cache.SetStringAsync(transaction.TransactionId.ToString(), serializedTransaction, options);
