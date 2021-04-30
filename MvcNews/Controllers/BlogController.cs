using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using aspNews.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcNews.Data;
using MvcNews.Models;

namespace MvcNews.Controllers
{
    public class BlogController : Controller
    {
        private readonly NewsDbContext _context;
        private readonly UserIdentityDbContext _identityContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<BlogController> _logger;
        private bool _isAdmin;
        private User _user;

        public BlogController(ILogger<BlogController> logger, NewsDbContext context, UserIdentityDbContext identityContext, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _identityContext = identityContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            
        }

        private void setUser()
        {
            Task<Models.User> user = _userManager.GetUserAsync(User);
            user.Wait();
            _user = user.Result;
            _isAdmin = _user.IsAdmin;
        }

        // posts
        public IActionResult CreatePost()
        {
            setUser();
            if (!_isAdmin)
                return RedirectToAction("Index", "Home");
            
            return View();
        }

        [HttpPost]
        public IActionResult CreatePost(postModel newPost)
        {
            setUser();
            if (!_isAdmin)
                return RedirectToAction("Index", "Home");
            if(!ModelState.IsValid)
                return View();

            Post post = new Post
            {
                Title = newPost.Title,
                Abstract = newPost.Abstract,
                Content = newPost.Content,
                CreationDate = DateTime.Now,
                // User = _user,
                //todo: set the image url here
                // Poster = newPost.Poster.FileName,
                Category = GetCategory(newPost.Category)
            };
            
            SetPostTags(newPost.Tags, post);
            
            _context.Posts.Add(post);
            _context.SaveChanges();
            
            return RedirectToAction("Index", "Home");
        }


        public class postModel
        {
            [Required]
            public string Title { get; set; }
            
            [Required]
            public string Abstract { get; set; }
            
            [Required]
            public string Content { get; set; }
            
            [Required]
            public string Category { get; set; }
            
            [Display(Name = "Tags", Prompt = "tags, separated, with, commas")]
            public string Tags { get; set; }
            
            public IFormFile Poster { set; get; }
        }

        private Category GetCategory(string name)
        {
            name = name.Trim();
            Category c = _context.Categories.FirstOrDefault(x => x.Name == name);
            
            if (c is not null)
                return c;

            c = new Category
            {
                Name = name
            };
            return c;
        }
        
        private void SetPostTags(string tagNames, Post post)
        {
            var tags = tagNames.Split(',').Select(x=>x.Trim());
            foreach (var t in tags)
            {
                Tag tag = GetTag(t);
                PostTag pt = new PostTag
                {
                    Post = post,
                    Tag = tag
                };
                _context.PostTags.Add(pt);
            }
        }
        
        private Tag GetTag(string name)
        {
            name = name.Trim();
            Tag c = _context.Tags.FirstOrDefault(x => x.Name == name);
            
            if (c is not null)
                return c;

            c = new Tag
            {
                Name = name
            };
            return c;
        }

    }
}