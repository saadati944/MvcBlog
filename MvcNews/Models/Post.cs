using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
        
        public string UserId { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<PostTag> PostTags { get; } = new();
    }
}