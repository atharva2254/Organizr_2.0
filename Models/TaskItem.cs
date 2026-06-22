using System.ComponentModel.DataAnnotations;

namespace Organizr.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? UserId { get; set; }
        public User? User { get; set; } = null!;
    }
}
