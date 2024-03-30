using Microsoft.EntityFrameworkCore;

namespace MyMarketplaceProject.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {

        }
        public TodoContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<TodoItem> TodoItems { get; set; } = null;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;user=admin;password=[REDACTED];database=todo",
                new MySqlServerVersion(new Version(8, 0, 36)));
        }
    }
}
