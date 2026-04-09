using System;
using PersonalProject.Models.ShoppingCartProcess;
using PersonalProject.Data;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Controllers;
using PayOS;// This is for the main PayOSClient
using PayOS.Models;// This is for CreatePaymentLinkRequest and WebhookData


namespace PersonalProject.Services;

public class PaymentService: IPaymentService
{   
    private readonly PersonalProjectContext _personalProjectContext;
    private readonly PayOSClient _payOSClient;

    public PaymentService(PersonalProjectContext personalProjectContext)
    {
        _personalProjectContext = personalProjectContext;
        // var clientId = config["PayOS:ClientId"] ?? throw new ArgumentNullException("PayOS:ClientId is missing in appsettings.json");
        // var apiKey = config["PayOS:ApiKey"] ?? throw new ArgumentNullException("PayOS:ApiKey is missing in appsettings.json");
        // var checksumKey = config["PayOS:ChecksumKey"] ?? throw new ArgumentNullException
        // ("PayOS:ChecksumKey is missing in appsettings.json");

        _payOSClient = new PayOSClient("", "", "0");
    }

    public async Task<string> CreatePaymentUrl(DTOOrder order)
    {

        // 1. Generate a unique Order code 
        long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        // 2. Create the payment link request
        var paymentLinkRequest = new PayOS.Models.V2.PaymentRequests.CreatePaymentLinkRequest
        {
            OrderCode = orderCode,
            Amount = order.TotalAmount,
            Description = $"Payment for Order {orderCode}",
            CancelUrl = "https://yourdomain.com/payment/cancel",
            ReturnUrl = "https://yourdomain.com/payment/success",
            // optional: you can add more fields here as needed by PayOS, such as customer info, etc.
            // Items = new List<OrderDetail>
            // {
            //     new OrderDetail()
            //     {
            //         ProductId = "1", // You can replace this with actual product ID
            //         ProductName = "Sample Product", // You can replace this with actual product name
            //         Quantity = 1, // You can replace this with actual quantity
            //         UnitPrice = order.TotalAmount // You can replace this with actual unit price
            //     }
                    
               
            // }
            // You can add more fields here as needed by PayOS
        };  
        // 3. Call PayOS to create the payment link
        var paymentLinkResult = await _payOSClient.PaymentRequests.CreateAsync(paymentLinkRequest);
        // Just access CheckoutUrl directly. If it fails, payOS will throw an Exception.
        Console.WriteLine($"PayOS Link Created: {paymentLinkResult.CheckoutUrl}");
        
        // 4. Return the payment URL to the caller
        return paymentLinkResult.CheckoutUrl;
    }

    public bool VerifyPaymentCallback(IDictionary<string, string> callbackData)
    {
        // // 1. Get the signature Momo sent
        // var receivedSignature = callbackData["signature"];
        // // 2. Calculate what the signature should be based on other data        
        // var myCalculatedSignature = CalculateHash(callbackData);
        // // 3. Compare the two signatures
        // return receivedSignature == myCalculatedSignature;
        return true; // For now, we will skip the actual implementation of signature verification
    }

    public async Task<string> GetTransactionStatus(string orderId)
    {
        // 1. Call PayOS API to get the transaction status
        // var transactionStatusResult = await _payOSClient.PaymentRequests.GetTransactionStatusAsync(orderId);

        var status = await _payOSClient.PaymentRequests.GetAsync(orderId);
        // Then you can check status.Status (e.g., "PAID", "PENDING")

        Console.WriteLine($"PayOS Get Transaction Status Result: {status.Status} ");
        // 2. Return the status to the caller
        return status.Status.ToString();
    }

    public string CreateSimpleVietQR(DTOOrder order)
{
    // 1. Your Bank Information (Replace with yours!)
    string bankId = "Sacombank"; // Example: Vietcombank (VCB)
    string accountNo = "0853833045"; 
    string accountName = "TRAN THUY OANH"; // Your full name
    string template = "compact2"; // Styles: 'qr_only', 'compact', 'compact2'

    // 2. Order Information
    int amount = (int)order.TotalAmount;
    string description = Uri.EscapeDataString($"First Journey Order {order.OrderId}"); // URL-encode the description

    // 3. Construct the URL
    // This URL returns a direct .png image of the QR code
    string qrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png" +
                   $"?amount={amount}" + 
                   $" &addInfo={description}" + 
                   $"&accountName={Uri.EscapeDataString(accountName)}";

    return qrUrl;
}




}
