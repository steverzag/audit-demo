using AuditDemo.API.Data.Models.Audit;
using UsersTasks.API.Data.Models;
using UsersTasks.API.DTOs;

namespace UsersTasks.API.Mapping
{
	public static class TaskMapping
	{
		public static TaskDTO ToDTO(this AppTask task)
		{
			return new TaskDTO
			(
				task.Id,
				task.Title,
				task.Description,
				task.CreatedAt,
				task.Priority,
				task.Status,
				task.IsCompleted,
				task.CompletedAt,
				task.UserId
			);
		}

		public static AppTask ToEntity(this CreateTaskRequest request)
		{
			return new AppTask
			{   
				Title = request.Title,
				Description = request.Description,
				Priority = request.Priority,
				Status = request.Status,
				UserId = request.UserId,
			};
		}

		public static AppTask ToEntity(this UpdateTaskRequest request, AppTask appTask)
		{
			appTask.Title = request.Title;
			appTask.Description = request.Description;
			appTask.Priority = request.Priority;
			appTask.Status = request.Status;
			appTask.UserId = request.UserId;

			return appTask;
		}

		public static AppTaskAuditDTO ToDTO(this AppTaskAudit audit)
		{
			return new AppTaskAuditDTO
			(
				audit.AuditOperation,
				audit.AuditedAt,
				audit.PreviousState,
				audit.EndingState,
				audit.Entity.ToDTO()
			);
		}
	}
}
