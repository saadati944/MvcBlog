using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcNews.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        
        public ICollection PostTags { get; } = new List<PostTag>();
    }
}
