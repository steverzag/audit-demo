using AuditDemo.API.Data.Enums;
using UsersTasks.API.Data.Enums;

namespace UsersTasks.API.DTOs
{
	public record TaskDTO
	(
		int Id,
		string Title,
		string? Description,
		DateTime CreatedAt,
		AppTaskPriority Priority,
		AppTaskStatus Status,
		bool IsCompleted,
		DateTime? CompletedAt,
		int? UserId
	);

	public record CreateTaskRequest
	(
		string Title,
		string Description,
		AppTaskPriority Priority,
		AppTaskStatus Status,
		int? UserId
	);

	public record UpdateTaskRequest
	(
		int Id,
		string Title,
		string Description,
		AppTaskPriority Priority,
		AppTaskStatus Status,
		int? UserId
	);

	public record AssignTaskUserRequest(int TaskId, int? UserId);

	public record AppTaskAuditDTO
	(
		AuditOperation AuditOperation,
		DateTime AuditedAt,
		string? PreviousState,
		string? EndingState,
		TaskDTO Task
	);
}
