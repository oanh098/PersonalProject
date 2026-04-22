using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moq;
using PersonalProject.Data;
using PersonalProject.Models.ShoppingCartProcess;
using PersonalProject.Services;



namespace PersonalProject.Controllers
{
// dotnet aspnet-codegenerator controller 
//-name ShoppingCartController 
//-m CartItem -dc PersonalProjectContext 
//--relativeFolderPath Controllers 
//--useDefaultLayout 
//--referenceScriptLibraries 

    public class ShoppingCartController : Controller
    {
        private readonly PersonalProjectContext _context;
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public ShoppingCartController(PersonalProjectContext context
        , ICartService cartService, IConfiguration configuration, IOrderService orderService
        , IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        
            _context = context;
            _cartService = cartService;
            _configuration = configuration;
        }

        
        // GET: ShoppingCart
        public async Task<IActionResult> Index()
        {
            ViewBag.DebugUser = User.Identity?.Name;
            ViewBag.DebugMerchant = HttpContext.Session.GetString("MerchantId");           
            
            var merchantId = HttpContext.Session.GetString("MerchantId");
            if(string.IsNullOrEmpty(merchantId))
            {
                return RedirectToAction("SelectStall");
            }

            var CartItemAsProduct = await _context.CartItem.ToListAsync();
            var ItemToPurchase = _cartService.GetCartAsync(merchantId, User.Identity?.Name ?? "anonymous").Result.Items;

            var viewModel = new ShoppingPageViewModel
            {
                CartItemAsProduct = CartItemAsProduct,
                ItemToPurchase = ItemToPurchase
            };
            return View(viewModel);
        }

        // GET: ShoppingCart/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: ShoppingCart/Create
        public IActionResult Create()
        {
            return View();
        }

        
        // POST: ShoppingCart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,ProductName,Quantity,Price")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cartItem);
        }

        // GET: ShoppingCart/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }

        // POST: ShoppingCart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ProductName,Quantity,Price")] CartItem cartItem)
        {
            if (id != cartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cartItem);
        }

        // GET: ShoppingCart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: ShoppingCart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        #region ShoppingProcess

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoShoppingCart([FromBody] 
        DTOAddToCartRequest request) 
        {
            var userId = User.Identity?.Name?? "anonymous" ; // Assuming user is authenticated
            var merchantId = HttpContext.Session.GetString("MerchantId") ?? "STALL_DEFAULT"; // Example of getting merchant ID from Session.Items["MerchantId"]?.ToString() ?? "default"; // Example of getting merchant ID from HttpContext    
            
            
            // using DTO to receive productId from AJAX request, 
            // then fetch product details from database

            var CartItemAsProduct = await _context.CartItem.FindAsync(request.Id);
           
            if (CartItemAsProduct == null)
            {
                return NotFound(new { message = "Item not found in catalog." });
            }
            var item = new ItemToPurchase
            {
                Quantity = request.Quantity, // Default quantity, can be modified to accept from request
                PricePerUnit = CartItemAsProduct.Price,
                ProductId = request.Id,
                Product = CartItemAsProduct

            };

            Console.WriteLine($"--- DEBUG INFO ---");
            Console.WriteLine($"User Name: {userId}");
            Console.WriteLine($"Is Authenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"Merchant ID from Context: {merchantId}");
            Console.WriteLine($"Quantity from request: {request.Quantity}");


            Console.WriteLine($"Merchant ID from HttpContext.Items: {merchantId}");
            Console.WriteLine($"------------------");
            
          
            // The [FromBody] attribute is required for AJAX JSON data
            var updatedCart = await _cartService.AddItemAsync(merchantId, userId, item);

            return Json(new  { 
                message = $"{CartItemAsProduct.ProductName} added!"
                , cart = updatedCart
                , success = true
                }); 
        }

        [HttpGet]
        public IActionResult SetStallMerchant(string stallId) 
        {
            HttpContext.Session.SetString("MerchantId", stallId);
            return RedirectToAction("Index", "ShoppingCart");
        }

        public IActionResult ChangeMerchant()
        {
            HttpContext.Session.Remove("MerchantId");
            return RedirectToAction("SelectStall");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePost([FromQuery] int Id) 
        {
            var userId = User.Identity?.Name?? "anonymous" ; // Assuming user is authenticated
           var merchantId = HttpContext.Session.GetString("MerchantId") ?? "STALL_DEFAULT";
            var updatedCart = await _cartService.RemoveItemAsync(merchantId, userId, Id);
            Console.WriteLine($"--- updatedCart INFO ---: {updatedCart.Items.Count}" );
            //  return Json(updatedCart); 

             return Json(new  { 
                message = $"Item removed!"
                , cart = updatedCart
                , success = true
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromForm] DTOCheckoutRequest request)
        {
            //// 1. Check if the DTO rules (Required, EmailAddress, etc.) are met
            if(!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"[CHECKOUT ERROR]: {error.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }

            var userId = User.Identity?.Name?? "anonymous" ; // Assuming user is authenticated
            string? merchantId = HttpContext.Session.GetString("MerchantId");
            if(string.IsNullOrEmpty(merchantId))
            {
                merchantId = _configuration["StoreSettings:MerchantId"] ?? "default";
            }

            // 2. Fetch the current cart for the user
            var cart = await _cartService.GetCartAsync(merchantId, userId);
            if (cart.Items.Count == 0)
            {
                return BadRequest(new { message = "Your cart is empty." });
            }

            // Here you would typically process the payment and create an order record in your database.
            // For this example, we'll just clear the cart and return a success message.

            // 3. USE THE ORDER SERVICE HERE
            // Create the order in the database, PostgreSQL
            // This is the core "Proprietary Logic" we discussed
            Order newOrder = await _orderService.CreateOrderAsync(request, cart, merchantId, userId);

            // 4. Generate the payment URL (e.g., Momo QR code) using the Payment Service
            // Map your local Order obj to the DTOOrder for the PaymentService expects
            var dtoOrder = new DTOOrder
            {
                OrderId = newOrder.Id,
                OrderInfo = $"FJ{newOrder.Id}",
                TotalAmount = (long)newOrder.TotalAmount, // Cast to long if needed
            };
            // string paymentUrl =  _paymentService.CreateSimpleVietQR(dtoOrder);


            // 5. Clear the cart after (Only if you are sure the user is moving to payment)
            // foreach (var item in cart.Items.ToList())
            // {
            //     await _cartService.RemoveItemAsync(merchantId, userId, item.ProductId);
            // }

            // 6. Return the QR payment URL to the frontend

            // return Json(new { 
            //     success = true,
            //     message = $"Checkout successful! Your order has been placed. Thank you, {request.FullName}!",
            //     redirectUrl = paymentUrl // This is now the VietQR page!
            //     });

            return Json(new { 
                success = true, 
                message = "Checkout successful!", 
                redirectUrl = Url.Action("Payment", "ShoppingCart", new { orderId = newOrder.Id }) 
            });

            //RedirectToAction("ActionName", "ControllerName")
           // return RedirectToAction("Payment", "ShoppingCart", new { orderId = newOrder.Id });

            // // Pass to the View
            // ViewBag.QrUrl = paymentUrl;
            // ViewBag.OrderId = paymentUrl; 
            // return View();
        }

        public async Task<IActionResult> Payment(string orderId)
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
            //return Ok(new { url = qrCodeUrl }); 

            ViewBag.QrUrl = qrCodeUrl;
            ViewBag.OrderId = orderId;
            return View();
        } 

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.Identity?.Name ?? "anonymous";
            var merchantId = HttpContext.Session.GetString("MerchantId") ?? "STALL_DEFAULT";

            // Re-use your existing service logic to fetch the current state
            var currentCart = await _cartService.GetCartAsync(merchantId, userId);
            
            // If the cart is null (empty cache), return a new empty cart object
            return Json(currentCart ?? new ShoppingCart());
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(string orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                // If the order isn't found, send them back home
                return RedirectToAction("Index", "Home");
            }

            // Pass the order to the view
            return View(order);
        }


        private bool CartItemExists(int id)
        {
            return _context.CartItem.Any(e => e.Id == id);
        }
            
        
        public IActionResult SelectStall()
        {
            // The code looks for Views/Home/SelectStall.cshtml
            return View(); 
        }
        #endregion
    }

    


    #region DTOShoppingCart     
    

    // DTOAddToCartRequest has ProductId and Quantity,
    public class DTOAddToCartRequest
    {
        public int Id { get; set; } 
        public int Quantity { get; set; } = 1; // Default quantity
        
    }


    //Its job is to capture what the user wants to buy (e.g., a list of IDs and quantities). 
    // It doesn't have a final price or an Order ID yet because the server hasn't created them.
    public class DTOCheckoutRequest
    {
        [Required (ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required (ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required (ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required (ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? DiscountCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = "COD"; // Default to Cash on Delivery

        public string? Note { get; set; } 
        
        // --- Cart Summary (Optional Security Check) ---
        // Some systems pass the Total here to verify against the server-calculated total
        public decimal ClientTotal { get; set; }

        // The Server gets it from the Login Cookie/Token (User.Identity.Name).
        // public string UserId { get; set; } = string.Empty;

        // The Server gets it from the Login Cookie/Token (User.Identity.Name).
        // public string MerchantId { get; set; } = string.Empty;

        // The Server pulls the Real Items from your Database/Cache using the UserId.
        // public List<DTOAddToCartRequest> Items { get; set; } = new List<DTOAddToCartRequest>();
    }

    /// <summary>
    /// using Records and Init is the standard "Pro" way 
    /// to ensure data doesn't change unexpectedly during a payment.
    /// follow this flow:
    ///Receive DTOCheckoutRequest.
    ///Save to Database to get a real OrderId.
    ///Map the saved data to a fresh DTOOrder.
    ///Send DTOOrder to MoMo.
    /// </summary>
    // DTOOrder (receipt, for payment) has the Price
    public class DTOOrder
    {
        // 1. Identification
        public int OrderId { get; init; }

        // 2. Financials
        ///int and long comes down to storage space and the maximum number they can hold.
        /// dealing with VND (Vietnamese Dong), this choice is very important 
        /// because currency values in Vietnam can become very large very quickly.
        public long TotalAmount { get; init; }
        public string Currency { get; init; } = "VND";


        // 3. Description (Show on Momo screen)
        public string OrderInfo { get; init; } = string.Empty;

        // 4. Items (Optional, but good for receipts)
        /// <summary>
        /// If your DTOAddToCartRequest only has ProductId and Quantity, 
        /// your receipt (the OrderDto) will be missing the Price.
        ///         DTOAddToCartRequest	                        OrderItemDto (in OrderDto)
        /// Purpose	What the user wants to put in the bag.	    What the user actually paid for.
        /// Price	Often missing (the server calculates it).   Must include the price at the time of purchase.
        /// </summary>
        public List<DTOOrderItem> Items { get; init; } = new();

        // 5. Timestamp
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;        
    }

    //: Use a separate OrderItemDto inside your OrderDto. 
    // This ensures that once the order is created, 
    // the price is "locked in." 
    // Even if you change the price of juice in your database tomorrow, 
    // the customer's receipt stays the same.
    public record DTOOrderItem(string ProductName, int Quantity, long Price);


    #endregion
}
