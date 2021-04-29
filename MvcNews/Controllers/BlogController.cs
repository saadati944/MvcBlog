using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MvcNews.Data;
using MvcNews.Models;

namespace MvcNews.Controllers
{
    public class BlogController : Controller
    {
        private readonly NewsDbContext _context;

        public BlogController(NewsDbContext context)
        {
            _context = context;
        }

        // categories
        [HttpGet]
        public IActionResult Categories()
        {
            return View(_context.Categories.ToList());
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCategory(Category newcCategory)
        {
            _context.Categories.Add(newcCategory);
            _context.SaveChanges();
            return RedirectToAction("Categories", "Blog");
        }
    }
}