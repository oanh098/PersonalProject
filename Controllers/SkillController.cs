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
    }
}