using System.Text.Json;

namespace AuditDemo.API.Utils
{
	public class JsonSerializerUtility
	{
		public static readonly JsonSerializerOptions AuditStateSerializerOptions = new()
		{
			TypeInfoResolver = new ExcludeClassPropertiesResolver()
		};
	}
}
