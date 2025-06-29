using System.Reflection;

namespace AuditDemo.API.Utils
{
	public static class SqlTypeHelper
	{
		private static readonly HashSet<Type> SqlCompatibleTypes = new()
		{
			typeof(Guid),
			typeof(DateTime),
			typeof(DateTimeOffset),
			typeof(TimeSpan),
			typeof(string), // For text
			typeof(byte[]) // For blobs
		};

		public static bool IsSqlCompatibleType(Type type)
		{
			// Handle nullable types (e.g., int?, DateTime?)
			if (Nullable.GetUnderlyingType(type) is Type underlyingType)
				type = underlyingType;

			return type.IsPrimitive || type.IsEnum || SqlCompatibleTypes.Contains(type);
		}

		public static IEnumerable<PropertyInfo> GetNonSqlCompatibleProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					   .Where(p => !IsSqlCompatibleType(p.PropertyType));
		}
	}
}
