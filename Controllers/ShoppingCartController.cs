using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public ShoppingCartController(PersonalProjectContext context
        , ICartService cartService, IConfiguration configuration, IOrderService orderService)
        {
            _orderService = orderService;
        
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
             

            return View(await _context.CartItem.ToListAsync());
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
        public async Task<IActionResult> AddPost([FromBody] DTOAddToCartRequest request) 
        {
            var userId = User.Identity?.Name?? "anonymous" ; // Assuming user is authenticated
            var merchantId = HttpContext.Session.GetString("MerchantId") ?? "STALL_DEFAULT"; // Example of getting merchant ID from Session.Items["MerchantId"]?.ToString() ?? "default"; // Example of getting merchant ID from HttpContext    
            // using DTO to receive productId from AJAX request, 
            // then fetch product details from database
            var product = await _context.CartItem.FindAsync(request.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            var item = new CartItem
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Quantity = request.Quantity, // Default quantity, can be modified to accept from request
                Price = product.Price
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
                message = $"{product.ProductName} added!"
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
        public async Task<IActionResult> RemovePost([FromQuery] string productId) 
        {
            var userId = User.Identity?.Name?? "anonymous" ; // Assuming user is authenticated
            var merchantId = "default"; // Replace with actual merchant ID if needed
            var updatedCart = await _cartService.RemoveItemAsync(merchantId, userId, productId);
            Console.WriteLine($"--- updatedCart INFO ---: {updatedCart.Items.Count}" );
            //  return Json(updatedCart); 

             return Json(new  { 
                message = $"Item removed!"
                , cart = updatedCart
                , success = true
                }
            );
        }

        public async Task<IActionResult> Checkout([FromBody] DTOCheckoutRequest request)
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

            // Fetch the current cart for the user
            var cart = await _cartService.GetCartAsync(merchantId, userId);
            if (cart.Items.Count == 0)
            {
                return BadRequest(new { message = "Your cart is empty." });
            }

            // Here you would typically process the payment and create an order record in your database.
            // For this example, we'll just clear the cart and return a success message.

            // 2. USE THE ORDER SERVICE HERE
            // This is the core "Proprietary Logic" we discussed
            Order newOrder = await _orderService.CreateOrderAsync(request, cart, merchantId, userId);

            // Clear the cart after checkout
            foreach (var item in cart.Items.ToList())
            {
                await _cartService.RemoveItemAsync(merchantId, userId, item.ProductId);
            }

            return Json(new { 
                success = true,
                message = $"Checkout successful! Your order has been placed. Thank you, {request.FullName}!",
                redirectUrl = "/Orders" // Example of redirect URL after checkout 
                });
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.Identity?.Name ?? "anonymous";
            var merchantId = "default";

            // Re-use your existing service logic to fetch the current state
            var currentCart = await _cartService.GetCartAsync(merchantId, userId);
            
            // If the cart is null (empty cache), return a new empty cart object
            return Json(currentCart ?? new ShoppingCart());
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
    

    // DTO
    public class DTOAddToCartRequest
    {
        public int ProductId { get; set; } 
        public int Quantity { get; set; } = 1; // Default quantity
        
    }

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
    #endregion
}
