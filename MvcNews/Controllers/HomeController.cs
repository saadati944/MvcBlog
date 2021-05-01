using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcNews.Data;
using MvcNews.Models;

namespace MvcNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly NewsDbContext _context;
        private User _user;

        public HomeController(UserManager<User> userManager, ILogger<HomeController> logger, NewsDbContext context)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        private void setUser()
        {
            Task<Models.User> user = _userManager.GetUserAsync(User);
            user.Wait();
            _user = user.Result;
            if (_user is not null)
            {
                ViewData["username"] = _user.UserName;
                ViewData["showsignin"] = false;
            }
        }

        public IActionResult Index()
        {
            setUser();
            return View(_context.Posts.OrderByDescending(x => x.CreationDate).Take(10).Include(x => x.Category)
                .Include(x => x.PostTags).ThenInclude(x => x.Tag).ToList());

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
    }
}