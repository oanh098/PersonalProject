using Microsoft.AspNetCore.Mvc;
using PersonalProject.Data;
using PersonalProject.Models;
using System.Linq;

namespace PersonalProject.Controllers
{
    public class SkillController : Controller
    {
        private readonly PersonalProjectContext _context;

        public SkillController(PersonalProjectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var skills = _context.Skill.ToList();
            return View("~/Views/Profile/Skills/Index.cshtml", skills);
        }

        public IActionResult Create()
        {
            return View("~/Views/Profile/Skills/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                // Ensure UTC for DateTime properties
                skill.CreatedAt = DateTime.SpecifyKind(skill.CreatedAt, DateTimeKind.Utc);
                skill.UpdatedAt = DateTime.SpecifyKind(skill.UpdatedAt, DateTimeKind.Utc);

                _context.Skill.Add(skill);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Profile/Skills/Create.cshtml", skill);
        }

        public IActionResult Edit(int id)
        {
            var skill = _context.Skill.Find(id);
            if (skill == null) return NotFound();
            return View("~/Views/Profile/Skills/Edit.cshtml", skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Skill skill)
        {
            if (id != skill.Id) return NotFound();
            if (ModelState.IsValid)
            {
                // Ensure UTC for DateTime properties
                skill.CreatedAt = DateTime.SpecifyKind(skill.CreatedAt, DateTimeKind.Utc);
                skill.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                _context.Update(skill);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Profile/Skills/Edit.cshtml", skill);
        }

        public IActionResult Delete(int id)
        {
            var skill = _context.Skill.Find(id);
            if (skill == null) return NotFound();
            return View("~/Views/Profile/Skills/Delete.cshtml", skill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var skill = _context.Skill.Find(id);
            if (skill != null)
            {
                _context.Skill.Remove(skill);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}