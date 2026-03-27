using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using PersonalProject.Models;
using PersonalProject.Data;
using PersonalProject.Models.PaymentAggregator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace PersonalProject.Controllers;

public class TransactionController : Controller
{
    private readonly PersonalProjectContext _context;
    private readonly IDistributedCache _cache;

    public TransactionController(PersonalProjectContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // Actions for handling transactions can be added here
    [HttpPost]
    public async Task<IActionResult> Create(decimal amount, string currency)
        {
             // 1. Create the Transaction object
            var transaction = new Transaction
            {
                Amount = amount,
                Currency = currency,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
                
            };
            // Before creating a new one, check if a similar recent transaction exists
            var existing = await _cache.GetStringAsync(transaction.TransactionId.ToString());
            if (existing != null) 
            {
                return BadRequest("A transaction is already in progress. Please wait.");
            }
           
            // 2. Save to Database SQL (Permanent Record)
            _context.Transaction.Add(transaction);
            await _context.SaveChangesAsync();

            // 3. Save to REDIS Cache (Fast Access)
            // We use the ID as the key to prevent double-processing
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };
            string serializedTransaction = JsonSerializer.Serialize(transaction);
            await _cache.SetStringAsync(transaction.TransactionId.ToString(), serializedTransaction, cacheOptions);
            return View("TransactionCreated", transaction);

            //ViewBag.CacheTimestamp = cacheData; co the su dung ViewBag de doc cached data
        }
    
}
