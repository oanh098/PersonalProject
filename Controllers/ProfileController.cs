using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using PersonalProject.Data;
using PersonalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalProject.Controllers;

public class ProfileController : Controller
{
    private readonly PersonalProjectContext _context;
    public ProfileController(PersonalProjectContext context)
    {
        _context = context;
    }
    // GET: /Profile/
    public IActionResult Index()
    {
        var skills = _context.Skill.ToList();
        var movies = _context.Movie.ToList();
        var portfolio = _context.Portfolio.ToList();
        var viewModels = new ProfileIndexViewModels
        {
            Skill = skills,
            Movie = movies,
            Portfolio = portfolio
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


