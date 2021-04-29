using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MvcNews.Models
{
    public class User : IdentityUser
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSuperUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}