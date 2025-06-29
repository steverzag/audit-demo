using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AuditDemo.API.Utils
{
	public class ExcludeClassPropertiesResolver : DefaultJsonTypeInfoResolver
	{
		public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
		{

			var jsonTypeInfo = base.GetTypeInfo(type, options);

			if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object)
			{
				for (int i = jsonTypeInfo.Properties.Count - 1; i >= 0; i--)
				{
					var property = jsonTypeInfo.Properties[i];
					if (!SqlTypeHelper.IsSqlCompatibleType(property.PropertyType))
					{
						jsonTypeInfo.Properties.RemoveAt(i);
					}
				}
			}

			return jsonTypeInfo;
		}
	}
}
