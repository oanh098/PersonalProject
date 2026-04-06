using System;
using PersonalProject.Controllers;
using PersonalProject.Models.ShoppingCartProcess;   
namespace PersonalProject.Services;


public interface IPaymentService
{
    /// <summary>
    /// Generates the payment URL (eg Momo QR Code/ Redirect link) for a given order.
    /// This is the "core" function of the payment service. 
    /// Technically, yes, you can reuse DTOCheckoutRequest, 
    /// but as a software engineer with seven years of experience, 
    /// you’ll likely find that it creates "tight coupling" 
    /// which might cause bugs later as First Journey grows.
    /// </summary>
    Task<string> CreatePaymentUrl(DTOOrder order);

    /// <summary>
    /// Verifies the signature/checksum "coming back" from the payment provider
    /// (e.g., Momo)
    /// </summary>
    bool VerifyPaymentCallback(IDictionary<string, string> callbackData);

    /// <summary>
    /// Queries the provider to check the current status of a transaction (e.g., Momo)
    /// </summary>
    Task<string> GetTransactionStatus(string orderId);

    string CreateSimpleVietQR(DTOOrder order);
}
