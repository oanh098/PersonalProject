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

        //IPN (Instant Payment Notification)
        [HttpPost("momo-ipn")]
        public async Task<IActionResult> MomoIPN([FromBody] IDictionary<string, string> data)
        {
            // 1. Verify that the "talk" is really from Momo
            var iValid = _paymentService.VerifyPaymentCallback(data);
            if (!iValid)
            {
                return BadRequest("Invalid signature");
            }
            // 2. Update order status in DB based on the callback data
            var orderId = data["orderId"];
            await _orderService.UpdateOrderStatusAsync(int.Parse(orderId), "Completed");

            // 3. Tell Momo "I got it" (They stop retrying to send the callback)"
            return NoContent();
        }
        
    }   

    
}
