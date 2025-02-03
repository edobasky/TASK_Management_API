using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Model;

namespace TaskManagement.API.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
    }
}
