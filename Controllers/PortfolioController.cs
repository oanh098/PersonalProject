using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalProject.Data;
using PersonalProject.Models;

namespace PersonalProject.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly PersonalProjectContext _context;

        public PortfolioController(PersonalProjectContext context)
        {
            _context = context;
        }

        // GET: Portfolio
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Profile/Portfolio/Index.cshtml",
                await _context.Portfolio.ToListAsync());
        }

        // GET: Portfolio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var portfolio = await _context.Portfolio.FirstOrDefaultAsync(m => m.Id == id);
            if (portfolio == null)
                return NotFound();

            return View("~/Views/Profile/Portfolio/Details.cshtml", portfolio);
        }

        // GET: Portfolio/Create
        public IActionResult Create()
        {
            return View("~/Views/Profile/Portfolio/Create.cshtml");
        }

        // POST: Portfolio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,Description,DetailsLink")] Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(portfolio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Profile/Portfolio/Create.cshtml", portfolio);
        }

        // GET: Portfolio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio == null)
                return NotFound();

            return View("~/Views/Profile/Portfolio/Edit.cshtml", portfolio);
        }

        // POST: Portfolio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,Description,DetailsLink")] Portfolio portfolio)
        {
            if (id != portfolio.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(portfolio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortfolioExists(portfolio.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Profile/Portfolio/Edit.cshtml", portfolio);
        }

        // GET: Portfolio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var portfolio = await _context.Portfolio.FirstOrDefaultAsync(m => m.Id == id);
            if (portfolio == null)
                return NotFound();

            return View("~/Views/Profile/Portfolio/Delete.cshtml", portfolio);
        }

        // POST: Portfolio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio != null)
            {
                _context.Portfolio.Remove(portfolio);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolio.Any(e => e.Id == id);
        }
    }
}
