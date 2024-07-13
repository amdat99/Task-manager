// Data/TaskManagerContext.cs
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
namespace TaskManager.Data
{
    public class TaskManagerDBContext(DbContextOptions<TaskManagerDBContext> options) : DbContext(options)
    {
        public DbSet<TaskModel> Task { get; set; }
    }
}
