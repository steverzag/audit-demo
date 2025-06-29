using AuditDemo.API.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditDemo.API.Data.Models.Audit
{

	public class AuditingEntity<T> : AuditingEntity where T : class, new()
	{
		public T Entity { get; set; } = new T();
	}

	public class AuditingEntity
	{
		public int AuditId { get; set; }
		public AuditOperation AuditOperation { get; set; }
		public DateTime AuditedAt { get; set; }
		public string? PreviousState { get; set; }
		public string? EndingState { get; set; }
	}

	public class DefaultEntity 
	{
	}
}
