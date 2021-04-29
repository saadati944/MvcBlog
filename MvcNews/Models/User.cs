using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MvcNews.Models
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSuperUser { get; set; }
        public bool IsAdmin { get; set; }

        public ICollection Posts { get; } = new List<Post>();
    }
}