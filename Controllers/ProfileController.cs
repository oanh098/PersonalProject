using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using PersonalProject.Data;
using PersonalProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace PersonalProject.Controllers;

public class ProfileController : Controller
{
    private readonly PersonalProjectContext _context;
    private readonly IDistributedCache _cacheProfile;
    public ProfileController(PersonalProjectContext context, IDistributedCache cacheProfile)
    {
        _context = context;
        _cacheProfile = cacheProfile;
    }
    // GET: /Profile/
    public async Task<IActionResult> Index()
    {
        string cacheKey = "ProfileIndexData";
        string? cacheData = await _cacheProfile.GetStringAsync(cacheKey);//"I know this might be null, and that's okay."

        if (string.IsNullOrEmpty(cacheData))
        {
            // Data not in cache, retrieve from database
            cacheData = "This is Profile cached data from the database at" + DateTime.Now ; // Placeholder to indicate data is cached
            // Save to Redis with an expiration time
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            await _cacheProfile.SetStringAsync(cacheKey, cacheData, options);
        }

        // --- option 1 Add the cache data to the ViewBag ---
        //ViewBag.CacheTimestamp = cacheData;

        var skills = await _context.Skill.ToListAsync();
        var movies = await _context.Movie.ToListAsync();
        var portfolio = await _context.PortfolioItem.ToListAsync();
        var viewModels = new ProfileIndexViewModels
        {
            Skill = skills,
            Movie = movies,
            Portfolio = portfolio,
            CacheData = cacheData//-- option 2 Add the cache data to the ViewModel --
        };
        return View(viewModels);
    }

    public IActionResult Create()
    {
        return View("Skills/Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Skill skill)
    {
        if (ModelState.IsValid)
        {
            _context.Skill.Add(skill);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View("Skills/Create",skill);
    }

    public IActionResult Edit(int id)
    {
        var skill = _context.Skill.Find(id);
        if (skill == null) return NotFound();
        return View("Skills/Edit",skill);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Skill skill)
    {
        if (id != skill.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(skill);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View("Skills/Edit",skill);
    }

    public IActionResult Details(int id)
    {
        var skill = _context.Skill.Find(id);
        if (skill == null) return NotFound();
        return View("Skills/Details", skill);
    }

    public IActionResult Delete(int id)
    {
        var skill = _context.Skill.Find(id);
        if (skill == null) return NotFound();
        return View("Skills/Delete", skill);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var skill = _context.Skill.Find(id);
        if (skill == null) return NotFound();

        _context.Skill.Remove(skill);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}


