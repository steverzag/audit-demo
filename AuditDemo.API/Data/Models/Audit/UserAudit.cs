using System.ComponentModel.DataAnnotations.Schema;
using UsersTasks.API.Data.Models;

namespace AuditDemo.API.Data.Models.Audit
{
	public class UserAudit : AuditingEntity<UserSnapshot>
	{
	}

	public class UserSnapshot : User
	{
	}
}
