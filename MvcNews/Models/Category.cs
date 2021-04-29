using System.Collections;
using System.Collections.Generic;

namespace MvcNews.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection Posts { get; } = new List<Post>();
    }
}