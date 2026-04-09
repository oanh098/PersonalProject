using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalProject.Services;

namespace PersonalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [HttpGet("checkout-qr/{orderId}")]
        public async Task<IActionResult> GetCheckoutQR(string orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null) return NotFound();

            var dtoOrder = new DTOOrder
            {
                
                TotalAmount = (long) order.TotalAmount,
            };
            // Generate the URL string
            string qrCodeUrl = _paymentService.CreateSimpleVietQR(dtoOrder);

            // You can return the URL to the frontend, and the frontend 
            // simply puts this in an <img src="..." /> tag.
            return Ok(new { url = qrCodeUrl });
        }
        
        [HttpPost("api/sepay-webhook")]
        public async Task<IActionResult> HandleSepayWebhook([FromBody] IDictionary<string, string> SePaydata)
        {
            // // 0. Security: Verify the callback is really from SePay (e.g., check signature, IP allowlist, etc.)
            // var authHeader = Request.Headers["Authorization"].ToString();
            // var expectedToken = "Bearer YOUR_COPIED_TOKEN_HERE";

            // if (string.IsNullOrEmpty(authHeader) || authHeader != expectedToken)
            // {
            //     return Unauthorized("Invalid Webhook Token");
            // }

            // 1. Find the order using 'code' or 'content' from SePay
            var orderId = await _orderService.GetOrderAsync(SePaydata["code"]);
            var amountPaid = decimal.Parse(SePaydata["amount"].ToString() ?? "0");

            // 2. Find the order in DB
            if (orderId != null && amountPaid >= orderId.TotalAmount)
            {
                // 3. Update order status based on the payment result
                await _orderService.UpdateOrderStatusAsync(orderId.Id, "Paid");
                return Ok(new { message = "Payment successful, order updated." });
            }
            // Handle Sepay webhook logic here
            return NoContent();
        }
    }   

    
}
