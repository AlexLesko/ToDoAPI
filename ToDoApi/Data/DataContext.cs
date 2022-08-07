using Microsoft.EntityFrameworkCore;
using ToDoApi.Model;

namespace ToDoApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Users> Users => Set<Users>();
        public DbSet<ToDoModel> ToDoModel => Set<ToDoModel>();
    }
}
