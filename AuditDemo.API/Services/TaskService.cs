using AuditDemo.API.Data.Models.Audit;
using Microsoft.EntityFrameworkCore;
using UsersTasks.API.Data;
using UsersTasks.API.DTOs;
using UsersTasks.API.Mapping;

namespace UsersTasks.API.Services
{
	public class TaskService
	{
		private readonly AppDBContext _dbContext;
		public TaskService(AppDBContext dbContext) 
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<IEnumerable<TaskDTO>> GetAllTasksAsync()
		{
			var tasks = await _dbContext.AppTasks
				.Select(e => e.ToDTO())
				.ToListAsync();

			return tasks;
		}

		public async Task<IEnumerable<TaskDTO>> GetTasksByUserIdAsync(int userId)
		{
			var tasks = await _dbContext.AppTasks
				.Where(t => t.UserId == userId)
				.Select(e => e.ToDTO())
				.ToListAsync();
			return tasks;
		}

		public async Task<TaskDTO?> GetTaskByIdAsync(int id)
		{
			var task = await _dbContext.AppTasks
				.Where(t => t.Id == id)
				.Select(e => e.ToDTO())
				.FirstOrDefaultAsync();
			return task;
		}

		public async Task<int> CreateTaskAsync(CreateTaskRequest request)
		{
			var task = request.ToEntity();
			await _dbContext.AppTasks.AddAsync(task);
			await _dbContext.SaveChangesAsync();
			return task.Id;
		}

		public async Task<TaskDTO?> UpdateTaskAsync(UpdateTaskRequest request)
		{
			var task = await _dbContext.AppTasks.FindAsync(request.Id);

			if (task is null)
			{
				return null;
			}

			request.ToEntity(task);
			_dbContext.AppTasks.Update(task);
			await _dbContext.SaveChangesAsync();

			return task.ToDTO();
		}

		public async Task<TaskDTO?> CompleteTaskAsync(int id)
		{
			var task = await _dbContext.AppTasks.FindAsync(id);
			if (task is null)
			{
				return null;
			}
			task.IsCompleted = true;
			task.CompletedAt = DateTime.UtcNow;
			_dbContext.AppTasks.Update(task);
			await _dbContext.SaveChangesAsync();
			return task.ToDTO();
		}

		public async Task<TaskDTO?> UnCompleteTaskAsync(int id)
		{
			var task = await _dbContext.AppTasks.FindAsync(id);
			if (task is null)
			{
				return null;
			}

			task.IsCompleted = false;
			task.CompletedAt = null;
			_dbContext.AppTasks.Update(task);

			await _dbContext.SaveChangesAsync();

			return task.ToDTO();
		}

		public async Task<TaskDTO?> AssignToUserAsync(AssignTaskUserRequest request)
		{ 
			var task = await _dbContext.AppTasks.FindAsync(request.TaskId);
			
			if (task is null)
			{
				return null;
			}

			var user = await _dbContext.Users.FindAsync(request.UserId);
			task.UserId = user?.Id;

			await _dbContext.SaveChangesAsync();

			return task.ToDTO();
		}

		public async Task DeleteTaskAsync(int id)
		{
			var task = await _dbContext.AppTasks.FindAsync(id);
			if (task is null)
			{
				return;
			}
			_dbContext.AppTasks.Remove(task);
			await _dbContext.SaveChangesAsync();
		}


		public async Task<IEnumerable<AppTaskAuditDTO>> GetAudits(int id)
		{
			return await _dbContext.TaskAudits
				.Where(e => e.Entity.Id == id)
				.OrderByDescending(e => e.AuditedAt)
				.Select(e => e.ToDTO())
				.ToListAsync();
		}

	}
}
