
using Microsoft.EntityFrameworkCore;
using UsersTasks.API.Data;
using UsersTasks.API.Data.Models;
using UsersTasks.API.DTOs;
using UsersTasks.API.Endpoints.Configuration;
using UsersTasks.API.Services;

namespace UsersTasks.API.Endpoints
{
	public class UserEndpoints : IEndpoints
	{
		public void RegisterEndpoints(IEndpointRouteBuilder builder)
		{
			var group = builder
				.MapGroup("/users")
				.WithTags("Users");

			group.MapGet("/", GetAllUsers);
			group.MapGet("/{id}", GetUserById).WithName("UserById");
			group.MapPost("/", CreateUser);
			group.MapPut("/", UpdateUser);
			group.MapDelete("/{id}", DeleteUser);
			group.MapGet("/{id}/audits", GetAudits);
		}

		private async static Task<IResult> GetAllUsers(UserService userService)
		{
			var users = await userService.GetAllUsersAsync();
			return Results.Ok(users);
		}

		private async static Task<IResult> GetUserById(int id, UserService userService)
		{
			var user = await userService.GetUserByIdAsync(id);
			if(user is null)
			{
				return Results.NotFound("user not found");
			}

			return Results.Ok(user);
		}

		private async static Task<IResult> CreateUser(CreateUserRequest request, UserService userService)
		{
			var userId = await userService.CreateUserAsync(request);
			return Results.CreatedAtRoute("UserById", routeValues: new { id = userId });
		}

		private async static Task<IResult> UpdateUser(UpdateUserRequest request, UserService userService)
		{
			var user = await userService.UpdateUserAsync(request);
			return Results.Ok(user);
		}

		private async static Task<IResult> DeleteUser(int id, UserService userService)
		{
			await userService.DeleteUserAsync(id);
			return Results.Ok("user deleted");
		}

		private async static Task<IResult> GetAudits(int id, UserService userService)
		{
			var result = await userService.GetAudits(id);
			return Results.Ok(result);
		}
	}
}
