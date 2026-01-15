using System;
using Microsoft.AspNetCore.Mvc;
using PersonalProject.Data;
using PersonalProject.Models.Visible;

namespace PersonalProject.Controllers;

public class VisibleController : Controller
{
    private readonly PersonalProjectContext _context;

    public VisibleController(PersonalProjectContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View("~/Views/Visible/Index.cshtml");
    }

 }
