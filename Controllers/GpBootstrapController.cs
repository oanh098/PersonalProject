using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Data;
using PersonalProject.Models.GpBootstrap;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace PersonalProject.Controllers
{
    public class GpBootstrapController : Controller
    {
        private readonly PersonalProjectContext _context;
        private readonly IDistributedCache _cache;

        public GpBootstrapController(PersonalProjectContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string key)
        {
            string cacheKey = $"data-{key}";
            byte[]? cachedData = null;
            try
            {
                cachedData = await _cache.GetAsync(cacheKey);
            }
            catch (Exception ex)
            {
                // Log cache retrieval error (optional)
                Console.WriteLine($"Cache retrieval error: {ex.Message}");
            }
            if (cachedData != null)
            {
                var cachedString = Encoding.UTF8.GetString(cachedData);
                return Ok($"From Cache: {cachedString}");
            }

            string dataFromDb = $"Data for {key} at {DateTime.Now}";
            var dataToCache = Encoding.UTF8.GetBytes(dataFromDb);
            var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            
            try // Try to set the cache (safe operation)
            {
                await _cache.SetAsync(cacheKey, dataToCache, cacheOptions);
            }
            catch (Exception ex)
            {
                // Log the exception, but don't prevent the return of the fresh DB data
                Console.WriteLine($"Redis Write Exception: {ex.Message}"); 
            }   

            return Ok($"From DB: {dataFromDb}");
        }

        // GET: GpBootstrap
        public async Task<IActionResult> Index()
        {
            return View(await _context.GpBootstrap.ToListAsync());
        }

        // GET: GpBootstrap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gpBootstrap = await _context.GpBootstrap
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gpBootstrap == null)
            {
                return NotFound();
            }

            return View(gpBootstrap);
        }

        // GET: GpBootstrap/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GpBootstrap/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] GpBootstrap gpBootstrap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gpBootstrap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gpBootstrap);
        }

        // GET: GpBootstrap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gpBootstrap = await _context.GpBootstrap.FindAsync(id);
            if (gpBootstrap == null)
            {
                return NotFound();
            }
            return View(gpBootstrap);
        }

        // POST: GpBootstrap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] GpBootstrap gpBootstrap)
        {
            if (id != gpBootstrap.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gpBootstrap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GpBootstrapExists(gpBootstrap.Id))
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
            return View(gpBootstrap);
        }

        // GET: GpBootstrap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gpBootstrap = await _context.GpBootstrap
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gpBootstrap == null)
            {
                return NotFound();
            }

            return View(gpBootstrap);
        }

        // POST: GpBootstrap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gpBootstrap = await _context.GpBootstrap.FindAsync(id);
            if (gpBootstrap != null)
            {
                _context.GpBootstrap.Remove(gpBootstrap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GpBootstrapExists(int id)
        {
            return _context.GpBootstrap.Any(e => e.Id == id);
        }
    }
}
