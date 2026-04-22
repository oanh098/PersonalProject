using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalProject.Services;
using PersonalProject.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.RegularExpressions;


namespace PersonalProject.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentController> _logger;

        private readonly IDistributedCache _cache;
        public PaymentController(IPaymentService paymentService, 
        IOrderService orderService, IDistributedCache cache, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("checkout-qr/{orderId}")]
        public async Task<IActionResult> GetCheckoutQR(string orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null) return NotFound();

            var dtoOrder = new DTOOrder
            {
                OrderId = order.Id,
                TotalAmount = (long) order.TotalAmount,
                OrderInfo = $"Thanh toán FJ{order.Id}"
            };
            // Generate the URL string
            string qrCodeUrl = _paymentService.CreateSimpleVietQR(dtoOrder);

            // You can return the URL to the frontend, and the frontend 
            // simply puts this in an <img src="..." /> tag.
            return Ok(new { url = qrCodeUrl });
        }


        //https://personalproject-a5zz.onrender.com/api/payment/sepay-webhook
        
        // [HttpPost("sepay-webhook")]
        // public async Task<IActionResult> HandleSepayWebhook([FromBody] IDictionary<string, string> SePaydata)
        // {
        //     // // 0. Security: Verify the callback is really from SePay (e.g., check signature, IP allowlist, etc.)
        //     // var authHeader = Request.Headers["Authorization"].ToString();
        //     // var expectedToken = "Bearer YOUR_COPIED_TOKEN_HERE";

        //     // if (string.IsNullOrEmpty(authHeader) || authHeader != expectedToken)
        //     // {
        //     //     return Unauthorized("Invalid Webhook Token");
        //     // }

        //     // 1. Find the order using 'code' or 'content' from SePay
        //     var orderId = await _orderService.GetOrderAsync(SePaydata["code"]);
        //     var amountPaid = decimal.Parse(SePaydata["amount"].ToString() ?? "0");

        //     // 2. Find the order in DB
        //     if (orderId != null && amountPaid >= orderId.TotalAmount)
        //     {
        //         // 3. Update order status based on the payment result
        //         await _orderService.UpdateOrderStatusAsync(orderId.Id, "Paid");
        //         return Ok(new { message = "Payment successful, order updated." });
        //     }
        //     // Handle Sepay webhook logic here
        //     return NoContent();
        // }


        [HttpPost("api/payment/sepay-webhook")]
        public async Task<IActionResult> ReceivePayment([FromBody] SeepayNotification data)
        {
            // // 1. Print to the server console so you can see it
            Console.WriteLine("--- SEPAY WEBHOOK RECEIVED ---");
            Console.WriteLine(data?.ToString());
            Console.WriteLine("------------------------------");
            _logger.LogInformation("Webhook Received. Content: {Content}", data?.Content);
            if (data == null) return BadRequest("No data received");
            if (string.IsNullOrWhiteSpace(data.Content))
            {
                _logger.LogWarning("Webhook received but 'Content' was null or empty.");
                return BadRequest("Content missing");
            }
            var match = Regex.Match(data.Content, @"FJ(\d+)");
    
            if (!match.Success)
            {
                _logger.LogWarning("Could not find Order ID in content: {Content}", data.Content);
                return BadRequest("Invalid content format");
            }

            // // 2. Return a 200 OK so SePay knows you got the message
            // return Ok(new { status = "success", message = "Data received by First Journey server" });

            // Create a temporary key based on the 'content' (Order ID) sent by the user
            // Example: "payment_status:FJ0123"

            string orderId = match.Value; // This will be "FJ40"
            string statusKey = $"payment_status:{orderId}"; 
            
            // Save "PAID" in the cache for 10 minutes
            await _cache.SetStringAsync(statusKey, "PAID", new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return Ok(new { status = "success" });
        
        }

        [HttpGet("api/payment/check-status/{orderId}")]
        public async Task<string> CheckStatus(string orderId)
        {
            var status = await _cache.GetStringAsync($"payment_status:{orderId}");
            return status ?? "PENDING";
            // return "PAID";
        }

          
    }   

    
}
