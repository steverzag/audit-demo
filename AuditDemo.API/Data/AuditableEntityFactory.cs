using AuditDemo.API.Data.Models.Audit;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AuditDemo.API.Data
{
	public class AuditableEntityFactory
	{
		public const string AUDIT_SHCEMA = "audit";
		public const string PROPERTY_NAME_PREFIX = "Entity_";
		public static AuditingEntity CreateAuditableEntityOfType(Type t)
		{

			var a = new AppTaskAudit();

			var method = typeof(AuditableEntityFactory).GetMethod(nameof(CreateAuditableEntityGeneric), BindingFlags.NonPublic | BindingFlags.Static);
			var generic = method!.MakeGenericMethod(t);
			return (AuditingEntity)generic.Invoke(null, null)!;
		}

		private static AuditingEntity CreateAuditableEntityGeneric<T>() where T : AuditingEntity
		{
			var t = typeof(T);
			var auditableEntity = (T)Activator.CreateInstance(t)!; //new AuditableEntity<T>();

			return auditableEntity;
		}

		// Candidate for generic method with 3 type parameters if stronger typing needed
		//private static AuditableEntity CreateAuditableEntityGeneric<T, T2, T3>() where T : AuditableEntity<T2> where T2 : class, T3, new() where T3 : class
		//{
		//	var t = typeof(T);
		//	var auditableEntity = (T)Activator.CreateInstance(t)!; //new AuditableEntity<T>();
		//
		//	return auditableEntity;
		//}

		public static void CastSnapshotEntityOfType(object snapshot, object entity)
		{
			var t = snapshot.GetType();
			var t2 = entity.GetType();
			var method = typeof(AuditableEntityFactory).GetMethod(nameof(CasttSnapshotEntityGeneric), BindingFlags.NonPublic | BindingFlags.Static);
			var generic = method!.MakeGenericMethod(t, t2);
			generic.Invoke(null, [snapshot, entity]);
		}

		private static void CasttSnapshotEntityGeneric<T, T2>(ref T snapshot, T2 entity) where T : class, T2 where T2 : class
		{
			foreach (var prop in entity.GetType().GetProperties())
			{
				var valueProp = snapshot.GetType().GetProperty(prop.Name);
				var valueToSet = prop.GetValue(entity);
				valueProp!.SetValue(snapshot, valueToSet);
			}
		}

		public static bool InheritsFromAuditableGeneric(Type typeToCheck)
		{
			var genericBaseType = typeof(AuditingEntity<>);

			if (typeToCheck.BaseType != null
				&& typeToCheck.BaseType.IsGenericType
				&& typeToCheck.BaseType.GetGenericTypeDefinition() == genericBaseType)
			{
				return true;
			}

			return false;
		}
	}
}
