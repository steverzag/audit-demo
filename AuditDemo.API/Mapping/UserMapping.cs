using AuditDemo.API.Data.Models.Audit;
using UsersTasks.API.Data.Models;
using UsersTasks.API.DTOs;

namespace UsersTasks.API.Mapping
{
	public static class UserMapping
	{
		public static UserDTO ToDTO(this User user)
		{
			return new UserDTO
			(
				user.Id,
				user.FirstName,
				user.LastName,
				user.Email,
				user.CreatedAt
			);
		}

		public static User ToEntity(this CreateUserRequest request)
		{
			return new User
			{
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email
			};
		}

		public static User ToEntity(this UpdateUserRequest request, User user)
		{
			user.FirstName = request.FirstName;
			user.LastName = request.LastName;
			user.Email = request.Email;

			return user;
		}

		public static UserAuditDTO ToDTO(this UserAudit userAudit)
		{ 
			return new UserAuditDTO
			(
				userAudit.AuditOperation,
				userAudit.AuditedAt,
				userAudit.PreviousState,
				userAudit.EndingState,
				userAudit.Entity.ToDTO()
			);
		}
	}
}
