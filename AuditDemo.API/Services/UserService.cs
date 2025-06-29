using AuditDemo.API.Data.Models.Audit;
using Microsoft.EntityFrameworkCore;
using UsersTasks.API.Data;
using UsersTasks.API.DTOs;
using UsersTasks.API.Mapping;

namespace UsersTasks.API.Services
{
	public class UserService
	{
		private readonly AppDBContext _dbContext;

		public UserService(AppDBContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<UserDTO?> GetUserByIdAsync(int id)
		{ 
			var user = await _dbContext.Users.FindAsync(id);
			if (user is null)
			{
				return null;
			}

			return user.ToDTO();
		}

		public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
		{ 
			var users = await _dbContext.Users
				.Select(e => e.ToDTO())
				.ToListAsync();

			return users;
		}

		public async Task<int> CreateUserAsync(CreateUserRequest request)
		{ 
			var user = request.ToEntity();
			await _dbContext.Users.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			return user.Id;
		}

		public async Task<UserDTO?> UpdateUserAsync(UpdateUserRequest request)
		{
			var user = await _dbContext.Users.FindAsync(request.Id);
			if (user is null)
			{
				return null;
			}

			request.ToEntity(user);
			_dbContext.Users.Update(user);

			await _dbContext.SaveChangesAsync();

			return user.ToDTO();
		}

		public async Task DeleteUserAsync(int id)
		{ 
			var user = await _dbContext.Users.FindAsync(id);
			if (user is null)
			{
				return;
			}

			_dbContext.Users.Remove(user);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<UserAuditDTO>> GetAudits(int id)
		{

			return await _dbContext.UserAudits.Where(e => e.Entity.Id == id)
				.OrderByDescending(e => e.AuditedAt)
				.Select(e => e.ToDTO())
				.ToListAsync();
		}
	}
}
