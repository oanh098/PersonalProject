using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Data;
using PersonalProject.Models.Restaurant;

namespace PersonalProject.Controllers
{
    public class RestaurantEntityController : Controller
    {
        private readonly PersonalProjectContext _context;

        public RestaurantEntityController(PersonalProjectContext context)
        {
            _context = context;
        }

        // GET: RestaurantEntity
        public async Task<IActionResult> Index()
        {
            return View(await _context.RestaurantEntity.ToListAsync());
        }

        // GET: RestaurantEntity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantEntity = await _context.RestaurantEntity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantEntity == null)
            {
                return NotFound();
            }

            return View(restaurantEntity);
        }

        // GET: RestaurantEntity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RestaurantEntity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,PhoneNumber,CuisineType,CreatedAt,UpdatedAt")] RestaurantEntity restaurantEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurantEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restaurantEntity);
        }

        // GET: RestaurantEntity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantEntity = await _context.RestaurantEntity.FindAsync(id);
            if (restaurantEntity == null)
            {
                return NotFound();
            }
            return View(restaurantEntity);
        }

        // POST: RestaurantEntity/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,PhoneNumber,CuisineType,CreatedAt,UpdatedAt")] RestaurantEntity restaurantEntity)
        {
            if (id != restaurantEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurantEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantEntityExists(restaurantEntity.Id))
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
            return View(restaurantEntity);
        }

        // GET: RestaurantEntity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantEntity = await _context.RestaurantEntity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantEntity == null)
            {
                return NotFound();
            }

            return View(restaurantEntity);
        }

        // POST: RestaurantEntity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurantEntity = await _context.RestaurantEntity.FindAsync(id);
            if (restaurantEntity != null)
            {
                _context.RestaurantEntity.Remove(restaurantEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantEntityExists(int id)
        {
            return _context.RestaurantEntity.Any(e => e.Id == id);
        }
    }
}
