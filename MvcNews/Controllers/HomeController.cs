using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcNews.Data;
using MvcNews.Models;

namespace MvcNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NewsDbContext _context;

        public HomeController(ILogger<HomeController> logger, NewsDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        
        
        // tags
        [HttpGet]
        public IActionResult Tags()
        {
            return View(_context.Tags.ToList());
        }

        [HttpGet]
        public IActionResult CreateTag()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateTag(Tag newtag)
        {
            _context.Tags.Add(newtag);
            _logger.LogInformation(newtag.Id.ToString());
            _logger.LogInformation(newtag.Name);
            _context.SaveChanges();
            _logger.LogInformation(_context.Tags.ToList().Count.ToString());
            return RedirectToAction("Tags", "Home");
        }
        
        // [HttpGet]
        // public IActionResult TagPosts(string? name)
        // {
        //     if (String.IsNullOrEmpty(name))
        //         return Tags();
        //
        //     return View();
        // }
    }
}