
using AuditDemo.API.Data.Enums;

namespace UsersTasks.API.DTOs
{
	public record UserDTO
	(
		int Id,
		string FirstName,
		string LastName,
		string Email,
		DateTime CreatedAt
	);

	public record CreateUserRequest
	(
		string FirstName,
		string LastName,
		string Email
	);

	public record UpdateUserRequest
	(
		int Id,
		string FirstName,
		string LastName,
		string Email
	);

	public record UserAuditDTO
	(
		AuditOperation AuditOperation,
		DateTime AuditedAt,
		string? PreviousState,
		string? EndingState,
		UserDTO User
	);
}
