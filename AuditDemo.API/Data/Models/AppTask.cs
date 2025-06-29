
using UsersTasks.API.Data.Enums;

namespace UsersTasks.API.Data.Models
{
	public class AppTask
	{
		public int Id { get; set; }
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public DateTime CreatedAt { get; set; }
		public AppTaskPriority Priority { get; set; }
		public AppTaskStatus Status { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime? CompletedAt { get; set; }
		public int? UserId { get; set; }
		public User? User { get; set; }
	}
}
