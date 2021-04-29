using System;
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
        public string Poster { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }
        
        public int UserId { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection PostTags { get; } = new List<PostTag>();
    }
}