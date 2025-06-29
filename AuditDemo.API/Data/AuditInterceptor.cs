using AuditDemo.API.Data.Enums;
using AuditDemo.API.Data.Models.Audit;
using AuditDemo.API.Services;
using AuditDemo.API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;
using System.Threading.Channels;
using UsersTasks.API.Data;

namespace AuditDemo.API.Data
{
	public class AuditInterceptor : SaveChangesInterceptor
	{
		private readonly Dictionary<AuditingEntity, EntityEntry> _auditableEntityEntries = [];
		private readonly Channel<AuditingChannelRequest> _auditingChannel;
		private const string EntityPropertyName = nameof(AuditingEntity<DefaultEntity>.Entity);

		public AuditInterceptor(Channel<AuditingChannelRequest> auditingChannel)
		{
			_auditingChannel = auditingChannel ?? throw new ArgumentNullException(nameof(auditingChannel));
		}

		public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			var context = eventData.Context;

			if (context is null)
			{
				return await base.SavingChangesAsync(eventData, result, cancellationToken);
			}

			var entries = context.ChangeTracker.Entries()
				.Where(e => (e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
					&& !typeof(AuditingEntity).IsAssignableFrom(e.Entity.GetType()));

			if (!entries.Any())
			{
				return await base.SavingChangesAsync(eventData, result, cancellationToken);
			}

			context.Model.GetEntityTypes()
				.Where(t => AuditableEntityFactory.InheritsFromAuditableGeneric(t.ClrType))
				.Join(entries,
					t => t.ClrType.GetProperty(EntityPropertyName)!.PropertyType.BaseType,
					e => e.Entity.GetType(),
					(t, e) =>
					{
						var snapShoptEntityType = t.ClrType.GetProperty(EntityPropertyName)!.PropertyType;
						var auditEntity = AuditableEntityFactory.CreateAuditableEntityOfType(t.ClrType);

						auditEntity.AuditOperation = e.State switch
						{
							EntityState.Added => AuditOperation.Create,
							EntityState.Modified => AuditOperation.Update,
							EntityState.Deleted => AuditOperation.Delete,
							_ => throw new InvalidOperationException("Unsupported entity state for auditing.")
						};

						var originalValues = e.OriginalValues.ToObject();

						auditEntity.PreviousState = e.State switch
						{
							EntityState.Added => null,
							_ => JsonSerializer.Serialize(originalValues, originalValues.GetType(), JsonSerializerUtility.AuditStateSerializerOptions)
						};

						return (auditEntity, entry: e);
					}
				)
				.ToList()
				.ForEach(e => _auditableEntityEntries.Add(e.auditEntity, e.entry));

			return await base.SavingChangesAsync(eventData, result, cancellationToken);
		}

		public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
		{
			if (_auditableEntityEntries.Count != 0)
			{
				await _auditingChannel.Writer.WriteAsync(new AuditingChannelRequest(_auditableEntityEntries), cancellationToken);
				_auditableEntityEntries.Clear();
			}

			return await base.SavedChangesAsync(eventData, result, cancellationToken);
		}

		public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
		{
			_auditableEntityEntries.Clear();
			await base.SaveChangesFailedAsync(eventData, cancellationToken);
		}
	}
}
