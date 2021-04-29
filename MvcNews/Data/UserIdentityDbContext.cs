using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MvcNews.Models;
using Microsoft.EntityFrameworkCore;

namespace aspNews.Data
{
    public class UserIdentityDbContext : IdentityDbContext<User>
    {
        public UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options): base(options) { }
        
        public override DbSet<User> Users { get; set; }
    }
}