using System.ComponentModel.DataAnnotations.Schema;
using UsersTasks.API.Data.Models;

namespace AuditDemo.API.Data.Models.Audit
{
	public class AppTaskAudit : AuditingEntity<AppTaskSnapshot>
	{
	}

	public class AppTaskSnapshot : AppTask
	{
		[NotMapped]
		public new User? User { get; set; }
	}
}
