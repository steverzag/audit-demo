
using AuditDemo.API.Data;
using AuditDemo.API.Data.Models.Audit;
using AuditDemo.API.Utils;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using System.Threading.Channels;
using UsersTasks.API.Data;

namespace AuditDemo.API.Services
{
	public class AuditingPersistProcessor : BackgroundService
	{
		private readonly IServiceProvider _services;
		private readonly Channel<AuditingChannelRequest> _auditingChannel;

		public AuditingPersistProcessor(IServiceProvider services, Channel<AuditingChannelRequest> auditingChannel)
		{
			_services = services;
			_auditingChannel = auditingChannel ?? throw new ArgumentNullException(nameof(auditingChannel));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (await _auditingChannel.Reader.WaitToReadAsync(stoppingToken))
			{
				var request = await _auditingChannel.Reader.ReadAsync(stoppingToken);
				if (request.CanBeProcessed)
				{
					await ProcessAuditing(request, stoppingToken);
				}
			}
		}

		private async Task ProcessAuditing(AuditingChannelRequest request, CancellationToken stoppingToken)
		{

			var auditableEntities = request.AuditableEntityEntries.Keys.ToList();

			foreach (var auditableEntity in auditableEntities)
			{
				var entityPropertyName = nameof(AuditingEntity<AuditingEntity>.Entity);
				var snapshot = auditableEntity.GetType().GetProperty(entityPropertyName)?.GetValue(auditableEntity)!;

				if (snapshot is null) continue;

				if (request.AuditableEntityEntries.TryGetValue(auditableEntity, out var entry))
				{
					var endingValues = entry.CurrentValues.ToObject();
					auditableEntity.EndingState = JsonSerializer.Serialize(entry.Entity, endingValues.GetType(), JsonSerializerUtility.AuditStateSerializerOptions);
					AuditableEntityFactory.CastSnapshotEntityOfType(snapshot, entry.Entity);
				}
			}

			using (var scope = _services.CreateScope())
			{
				var context =
					scope.ServiceProvider
						.GetRequiredService<AppDBContext>();

				// Ensure that the auditable entities are of the correct type
				auditableEntities = auditableEntities.Join(
					context.Model.GetEntityTypes(),
					a => a.GetType(),
					t => t.ClrType,
					(a, _) => a)
					.ToList();

				await context.AddRangeAsync(auditableEntities, stoppingToken);
				await context.SaveChangesAsync(stoppingToken);
			}
		}
	}

	public record AuditingChannelRequest(IDictionary<AuditingEntity, EntityEntry> AuditableEntityEntries)
	{
		public IDictionary<AuditingEntity, EntityEntry> AuditableEntityEntries { get; set; } = AuditableEntityEntries?.ToDictionary() ?? [];
		public bool CanBeProcessed { get => AuditableEntityEntries is not null && AuditableEntityEntries.Count != 0; }
	}
}
