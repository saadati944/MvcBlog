using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcNews.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Content { get; set; }

        public IList Tags { get; } = new List<Tag>();
    }
}