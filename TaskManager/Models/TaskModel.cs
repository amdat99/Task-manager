// Models/Task.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class TaskModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100 , ErrorMessage = "Title cannot be more than 100 characters")]
        [Required (ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000 , ErrorMessage = "Description cannot be more than 1000 characters")]
        public string? Description { get; set; }

        public bool Completed { get; set; } = false;
        
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
