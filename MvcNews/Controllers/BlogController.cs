using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
        private User _user;

        public BlogController(ILogger<BlogController> logger, NewsDbContext context,
            UserIdentityDbContext identityContext, UserManager<User> userManager, SignInManager<User> signInManager)
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
            if (_user is not null)
            {
                ViewData["username"] = _user.UserName;
                if(_user.IsSuperUser)
                    ViewData["userisadmin"] = true;
                ViewData["showsignin"] = false;
            }
        }

        #region CreatePost

        public IActionResult CreatePost()
        {
            setUser();
            if (!_user.IsAdmin)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult CreatePost(PostModel newPost)
        {
            setUser();
            if (!_user.IsAdmin)
                return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid)
                return View();

            Post post = new Post
            {
                Title = newPost.Title,
                Abstract = newPost.Abstract,
                Content = newPost.Content,
                CreationDate = DateTime.Now,
                UserId = _user.Id,
                Poster = getPosterPath(newPost.Poster),
                Category = GetCategory(newPost.Category)
            };

            SetPostTags(newPost.Tags, post);

            _context.Posts.Add(post);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }


        public class PostModel
        {
            [Required] public string Title { get; set; }

            [Required] public string Abstract { get; set; }

            [Required] public string Content { get; set; }

            [Required] public string Category { get; set; }

            [Display(Name = "Tags", Prompt = "tags, separated, with, commas")]
            public string Tags { get; set; }

            public IFormFile Poster { set; get; }
        }

        private string getPosterPath(IFormFile file)
        {
            if (file is null)
                return "";

            string relationalPath = Path.Combine(Program.UploadPath, file.FileName);

            using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relationalPath),
                FileMode.Create))
                file.CopyToAsync(stream).Wait();

            return relationalPath;
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
            var tags = tagNames.Split(',').Select(x => x.Trim());
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

        #endregion

        public IActionResult Post(int id)
        {
            ViewData["showsignin"] = false;
            Post p = _context.Posts.Include(x => x.Category)
                .Include(x => x.PostTags).ThenInclude(x => x.Tag).FirstOrDefault(p => p.Id == id);

            User author = _identityContext.Users.FirstOrDefault(x => x.Id == p.UserId);

            if (author is not null)
                ViewData["AuthorName"] = author.UserName;
            else
                ViewData["AuthorName"] = "Unknown";

            if (p is null)
                return RedirectToAction("Index", "Home");

            return View(p);
        }

        public IActionResult Tags()
        {
            ViewData["showsignin"] = false;
            return View(_context.Tags.Include(x => x.PostTags).ThenInclude(x => x.Post).ToList());
        }

        public IActionResult Categories()
        {
            ViewData["showsignin"] = false;
            return View(_context.Categories.Include(x => x.Posts).ToList());
        }

        public IActionResult MyPosts(string? id)
        {   
            if (id is not null)
            {
                _user = _identityContext.Users.Single(x => x.Id == id);
            }
            if(_user is null)
            {
                setUser();
                if (!_user.IsAdmin)
                    return RedirectToAction("Index", "Home");
            }
            return View(_context.Posts.Where(x => x.UserId == _user.Id).OrderByDescending(x => x.CreationDate)
                .Include(x => x.PostTags).ThenInclude(x => x.Tag)
                .Include(x => x.Category));
        }

        public IActionResult RemovePost(int? id)
        {
            setUser();
            if (!_user.IsAdmin || id is null)
                return RedirectToAction("Index", "Home");
            Post p = _context.Posts.Include(x => x.Category).FirstOrDefault(x => x.Id == id);
            if (p is null)
                return RedirectToAction("MyPosts", "Blog");
            ViewData["post"] = p;
            return View(new ConfirmPostRemovingModel {Sure = true, Id = p.Id});
        }

        public IActionResult RemovePostConfirm(ConfirmPostRemovingModel confirmed)
        {
            setUser();
            if (!_user.IsAdmin || !confirmed.Sure)
                return RedirectToAction("Index", "Home");
            Post p = _context.Posts.Include(x => x.Category).FirstOrDefault(x => x.Id == confirmed.Id);
            if (p is null)
                return RedirectToAction("MyPosts", "Blog");

            _context.Remove(p);
            _context.SaveChanges();

            return RedirectToAction("MyPosts", "Blog");
        }

        public class ConfirmPostRemovingModel
        {
            [HiddenInput] public bool Sure { get; set; } = true;
            [HiddenInput] public int Id { get; set; }
        }

        public IActionResult EditPost(int? id)
        {
            setUser();
            if (!_user.IsAdmin || id is null)
                return RedirectToAction("Index", "Home");
            Post p = _context.Posts.Include(x => x.Category).Include(x => x.PostTags).ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (p is null)
                return RedirectToAction("MyPosts", "Blog");
            var pm = new PostModel
            {
                Title = p.Title,
                Abstract = p.Abstract,
                Content = p.Content,
                Category = p.Category.Name,
                Tags = String.Join(", ", p.PostTags.Select(x => x.Tag.Name))
            };
            ViewData["postid"] = id;
            return View(pm);
        }
        
        [HttpPost]
        public IActionResult EditPost(int? id, PostModel postModel)
        {
            setUser();
            if (!_user.IsAdmin || id is null)
                return RedirectToAction("Index", "Home");
            
            Post p = _context.Posts.Include(x => x.Category).Include(x => x.PostTags).ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (p is null)
                return RedirectToAction("MyPosts", "Blog");
            
            setUser();
            if (!_user.IsAdmin)
                return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid)
                return View();

            p.Title = postModel.Title;
            p.Abstract = postModel.Abstract;
            p.Content = postModel.Content;
            // CreationDate = DateTime.Now;
            // UserId = _user.Id;
            p.Poster = postModel.Poster is not null ? getPosterPath(postModel.Poster) : p.Poster;
            p.Category = GetCategory(postModel.Category);

            foreach (PostTag pt in p.PostTags)
            {
                _context.Remove(pt);
            }
            
            // _context.SaveChanges();
            p.PostTags.Clear();
            
            SetPostTags(postModel.Tags, p);

            _context.SaveChanges();

            return RedirectToAction("MyPosts");
            
        }
    }
}