using System.ComponentModel.DataAnnotations;

namespace MvcNews.Models
{
    public class PostTag
    {
        [Key]
        public int Id { get; set; }
        
        public int PostId { get; set; }
        public int TagId { get; set; }
        
        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}