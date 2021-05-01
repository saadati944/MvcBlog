using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using aspNews.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcNews.Models;

namespace MvcNews.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserIdentityDbContext _context;
        private User _user;

        public AccountController(UserIdentityDbContext context, UserManager<User> userManager, SignInManager<User> signInManager) : base()
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        
        private void setUser()
        {
            Task<Models.User> user = _userManager.GetUserAsync(User);
            user.Wait();
            _user = user.Result;
            if (_user is not null)
            {
                ViewData["username"] = _user.UserName;
                if(_user.IsSuperUser)
                    ViewData["userisadmin"] = true;
                ViewData["showsignin"] = false;
            }
        }

        //logout
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginInModel loginData)
        {
            if(!ModelState.IsValid)
                return View();
            
            var result = await _signInManager.PasswordSignInAsync(loginData.Username, loginData.Password, loginData.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
            
            return RedirectToAction("Index", "Home");
        }

        public class LoginInModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
        
        // register new user
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel newUser)
        {
            if(!ModelState.IsValid)
                return View();

            Models.User user = new User
            {
                UserName = newUser.UserName,
                Email = newUser.Email
            };
            if (_context.Users.Any())
                user.IsSuperUser = true;
            
            //todo: only admins can create posts.
            user.IsAdmin = true;
            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }
            
            return RedirectToAction("Login");
        }

        public class SignUpModel
        {
            [Required]
            [StringLength(130, ErrorMessage = "Max username len is 130 characters.")]
            public string UserName { get; set; }
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public IActionResult ShowUsers()
        {
            setUser();
            if (_user is null || !_user.IsSuperUser)
                return RedirectToAction("Index", "Home");


            return View(_context.Users.ToList());
        }
    }
}