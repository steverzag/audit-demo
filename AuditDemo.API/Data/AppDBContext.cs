using AuditDemo.API.Data;
using AuditDemo.API.Data.Models.Audit;
using AuditDemo.API.Migrations.CustomOperations;
using AuditDemo.API.Services;
using AuditDemo.API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Channels;
using UsersTasks.API.Data.Models;

namespace UsersTasks.API.Data
{
	public class AppDBContext : DbContext
	{
		private readonly AuditInterceptor _auditInterceptor;

		public AppDBContext(DbContextOptions<AppDBContext> options, Channel<AuditingChannelRequest> auditingChannel) : base(options)
		{
			_auditInterceptor = new AuditInterceptor(auditingChannel ?? throw new ArgumentNullException(nameof(auditingChannel)));
		}

		public DbSet<User> Users { get; set; }
		public DbSet<AppTask> AppTasks { get; set; }

		//Auditable Entities
		public DbSet<UserAudit> UserAudits { get; set; }
		public DbSet<AppTaskAudit> TaskAudits { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder
				.AddInterceptors(_auditInterceptor)
				.ReplaceService<IMigrationsSqlGenerator, AppMigrationSqlGenerator>();
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(builder =>
			{
				builder.HasKey(u => u.Id);

				builder.Property(e => e.CreatedAt)
					.HasDefaultValueSql("GETDATE()");

				builder.HasMany(u => u.Tasks)
					.WithOne(t => t.User)
					.HasForeignKey(t => t.UserId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			modelBuilder.Entity<AppTask>(builder =>
			{
				builder.Property(e => e.Priority);
				builder.HasKey(u => u.Id);

				builder.Property(e => e.CreatedAt)
					.HasDefaultValueSql("GETDATE()");
			});

			ConfigureAuditableEntities(modelBuilder);
		}

		private static void ConfigureAuditableEntities(ModelBuilder modelBuilder)
		{
			var auditableEntities = modelBuilder.Model
				.GetEntityTypes()
				.Where(t => AuditableEntityFactory.InheritsFromAuditableGeneric(t.ClrType));

			foreach (var auditableEntity in auditableEntities)
			{
				ModelAuditableEntity(modelBuilder, auditableEntity);
			}
		}

		private static void ModelAuditableEntity(ModelBuilder modelBuilder, IMutableEntityType auditableEntity)
		{
			var entityPropertyName = nameof(AuditingEntity<DefaultEntity>.Entity);
			var entityPropertyType = auditableEntity.ClrType.GetProperty(entityPropertyName)
				?? throw new InvalidOperationException($"The Entity property could not be found in {auditableEntity.ClrType.Name} Type.");

			modelBuilder.Entity(auditableEntity.ClrType)
					.ToTable(auditableEntity.ClrType.Name, AuditableEntityFactory.AUDIT_SHCEMA);

			modelBuilder.Entity(auditableEntity.ClrType)
				.HasKey(nameof(AuditingEntity.AuditId));

			modelBuilder.Entity(auditableEntity.ClrType)
				.Property(nameof(AuditingEntity.AuditedAt))
				.HasDefaultValueSql("GETDATE()");

			modelBuilder.Entity(auditableEntity.ClrType).Ignore(entityPropertyName);

			modelBuilder.Entity(auditableEntity.ClrType).OwnsOne(entityPropertyType.PropertyType, entityPropertyName, ownedNavigatorBuilder =>
			{
				var nonSqlProps = SqlTypeHelper.GetNonSqlCompatibleProperties(ownedNavigatorBuilder.OwnedEntityType.ClrType);

				foreach (var nonSqlProp in nonSqlProps)
				{
					ownedNavigatorBuilder.OwnedEntityType.AddIgnored(nonSqlProp.Name);
				}

				var ownedEntityProperties = ownedNavigatorBuilder.OwnedEntityType.ClrType.GetProperties()
					.Where(p => !p.IsDefined(typeof(NotMappedAttribute)))
					.Select(p => p.Name)
					.ToList();

				var props = ownedNavigatorBuilder.OwnedEntityType
				.GetProperties()
				.ToList();

				props.ForEach(p =>
				{
					if (p.IsForeignKey())
					{
						var keys = p.GetContainingForeignKeys();
						foreach (var key in keys)
						{
							var mappedProperty = nonSqlProps
							.FirstOrDefault(p => p.PropertyType == key.PrincipalEntityType.ClrType
								&& !p.IsDefined(typeof(NotMappedAttribute)));
							if (mappedProperty != null)
							{
								throw new InvalidOperationException(
									$"The property '{mappedProperty.Name}' on type '{ownedNavigatorBuilder.OwnedEntityType.ClrType.Name}' is part of a relationship defined on it self or its base type. " +
									$"To avoid mapping errors, please decorate this property with the [NotMapped] attribute, or review your entity relationships to ensure only SQL-compatible types are mapped.");
							}
						}
					}

					var propertyBuilder = ownedNavigatorBuilder.Property(p.Name)
						.IsRequired(p.IsNullable == false);

					var matchingProperty = ownedEntityProperties
						.FirstOrDefault(ownedP => p.Name == ownedP);

					if (matchingProperty != null)
						propertyBuilder.HasColumnName(AuditableEntityFactory.PROPERTY_NAME_PREFIX + p.Name);
				});
			});
		}
	}
}
