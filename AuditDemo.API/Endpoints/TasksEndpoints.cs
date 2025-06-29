using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using UsersTasks.API.DTOs;
using UsersTasks.API.Endpoints.Configuration;
using UsersTasks.API.Services;

namespace UsersTasks.API.Endpoints
{
	public class TasksEndpoints : IEndpoints
	{
		public void RegisterEndpoints(IEndpointRouteBuilder app)
		{
			var group = app
				.MapGroup("/tasks")
				.WithTags("Tasks")
				.AddFluentValidationAutoValidation();

			group.MapGet("/", GetAllTasks);
			group.MapGet("/by-user/{userId}", GetTasksByUser);
			group.MapGet("/{id}", GetTaskById).WithName("TaskById");
			group.MapPost("/", CreateTask);
			group.MapPut("/", UpdateTask);
			group.MapDelete("/{id}", DeleteTask);
			group.MapPatch("/{id}/complete", CompleteTask);
			group.MapPatch("/{id}/uncomplete", UnCompleteTask);
			group.MapPatch("/assign-user", AssignToUser);
			group.MapGet("/{id}/audits", GetAudits);
		}

		private async Task<IResult> AssignToUser(AssignTaskUserRequest request, TaskService taskService)
		{
			var task = await taskService.AssignToUserAsync(request);

			if (task is null)
			{
				return Results.NotFound("Task to be assigned not found");
			}

			return Results.Ok(task);
		}

		private async Task<IResult> UnCompleteTask(int id, TaskService taskService)
		{
			var task = await taskService.UnCompleteTaskAsync(id);

			if (task is null)
			{
				return Results.NotFound("Task to be uncompleted not found");
			}

			return Results.Ok("Task uncompleted");
		}

		private async Task<IResult> CompleteTask(int id, TaskService taskService)
		{
			var task = await taskService.CompleteTaskAsync(id);

			if(task is null)
			{
				return Results.NotFound("Task to be completed not found");
			}

			return Results.Ok("Task completed");
		}

		private async Task<IResult> DeleteTask(int id, TaskService taskService)
		{
			await taskService.DeleteTaskAsync(id);

			return Results.Ok("Task deleted");
		}

		private async Task<IResult> UpdateTask(UpdateTaskRequest request, TaskService taskService)
		{
			var task = await taskService.UpdateTaskAsync(request);

			if (task is null)
			{
				return Results.NotFound("Task to be updated not found");
			}

			return Results.Ok(task);
		}

		private async Task<IResult> CreateTask(CreateTaskRequest request, TaskService taskService)
		{
			var taskId = await taskService.CreateTaskAsync(request);

			if (taskId == 0)
			{
				return Results.BadRequest("Task creation failed");
			}

			return Results.CreatedAtRoute("TaskById", routeValues: new { id = taskId });
		}

		private async Task<IResult> GetTaskById(int id, TaskService taskService)
		{
			var task = await taskService.GetTaskByIdAsync(id);
			if (task is null)
			{
				return Results.NotFound("Task not found");
			}

			return Results.Ok(task);
		}

		private async Task<IResult> GetTasksByUser(int userId, TaskService taskService)
		{
			var userTasks = await taskService.GetTasksByUserIdAsync(userId);

			return Results.Ok(userTasks);
		}

		private async Task<IResult> GetAllTasks(TaskService taskService)
		{
			var tasks = await taskService.GetAllTasksAsync();

			return Results.Ok(tasks);
		}

		private async Task<IResult> GetAudits(int id, TaskService taskService)
		{
			var result = await taskService.GetAudits(id);

			return Results.Ok(result);
		}
	}
}
