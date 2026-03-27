using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PersonalProject.Data; 
using System.Text.Json;
using PersonalProject.Models.PaymentAggregator;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace PersonalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsApiController : ControllerBase
    {
        private readonly PersonalProjectContext _context;
        private readonly IDistributedCache _cache;

        public TransactionsApiController(PersonalProjectContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequest request)
        {
            // Before creating a new one, check if a similar recent transaction exists
            var existing = await _cache.GetStringAsync(request.TransactionId.ToString());
            if (existing != null)
            {
                return BadRequest("A transaction is already in progress. Please wait.");
            }
            // 1. Create the Transaction object
            var transactionModel = new Transaction
            {
                Amount = request.Amount,
                Currency = request.Currency ?? "USD",
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                MerchantId = request.MerchantId
            };
            // 2. Save to Database SQL (Permanent Record)
            _context.Transaction.Add(transactionModel);
            await _context.SaveChangesAsync();

            // 3. Save to REDIS Cache (Fast Access)
            // We use the ID as the key to prevent double-processing
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };
            string jsonData = JsonSerializer.Serialize(transactionModel);
            await _cache.SetStringAsync($"txn_{transactionModel.TransactionId}", jsonData, cacheOptions);
            
            // Add the cache data to the ViewBag
            // Return JSON instead of a View
            //"I have successfully saved this transaction 
            // to the Database and Redis. 
            // Here is the data I saved, and here is the ID 
            // you can use to look it up later."
            return CreatedAtAction(nameof(CreateTransaction), 
            new { id = transactionModel.TransactionId }, transactionModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetStatus(Guid id)
        {
            // Try to get from cache first
            var cachedData = await _cache.GetStringAsync($"txn_{id}");
            if (cachedData != null)
            {
                var cachedTransaction = JsonSerializer.Deserialize<Transaction>(cachedData);
                return Ok(cachedTransaction);
            }

            // Fallback to database
            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        public class TransactionRequest
        {
            public Guid TransactionId { get; set; } = Guid.NewGuid();
            public decimal Amount { get; set; }
            public string? Currency { get; set; }
            public string? MerchantId { get; set; }
        }


    }
}