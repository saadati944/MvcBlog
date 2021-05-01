using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MvcNews.ViewModels
{
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
}