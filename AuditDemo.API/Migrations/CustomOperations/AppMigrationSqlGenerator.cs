using AuditDemo.API.Data;
using AuditDemo.API.Data.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Update;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuditDemo.API.Migrations.CustomOperations
{
	public class AppMigrationSqlGenerator : SqlServerMigrationsSqlGenerator
	{
		private readonly string _activeProvider;

		public AppMigrationSqlGenerator(MigrationsSqlGeneratorDependencies dependencies,
			ICommandBatchPreparer commandBatchPreparer) : base(dependencies, commandBatchPreparer)
		{
		}

		protected override void Generate(
		MigrationOperation operation,
		IModel model,
		MigrationCommandListBuilder builder)
		{
			if (operation is RenameColumnOperation renameColumnOperation)
			{
				GenerateRename(renameColumnOperation, builder);
			}
			else if (operation is DropColumnOperation dropColumnOperation)
			{
				GenerateDrop(dropColumnOperation, builder);
			}
			else if (operation is AddColumnOperation addColumnOperation)
			{
				GenerateAdd(addColumnOperation, builder);
			}

			base.Generate(operation, model, builder);
		}

		private static void GenerateRename(
		RenameColumnOperation renameColumnOperation,
		MigrationCommandListBuilder builder)
		{
			if (renameColumnOperation.Schema is not AuditableEntityFactory.AUDIT_SHCEMA
				|| !renameColumnOperation.NewName.StartsWith(AuditableEntityFactory.PROPERTY_NAME_PREFIX))
			{ 
				return;
			}

			var oldPropertyName = renameColumnOperation.Name[AuditableEntityFactory.PROPERTY_NAME_PREFIX.Length..];
			var newPropertyName = renameColumnOperation.NewName[AuditableEntityFactory.PROPERTY_NAME_PREFIX.Length..];

			var previousState = nameof(AuditingEntity.PreviousState);
			var endingState = nameof(AuditingEntity.EndingState);

			builder.AppendLine($"UPDATE [{renameColumnOperation.Schema}].[{renameColumnOperation.Table}]")
				.AppendLine($"SET [{previousState}] = JSON_MODIFY([{previousState}], '$.{newPropertyName}', " +
					$"COALESCE(JSON_VALUE([{previousState}], '$.{oldPropertyName}'), ''));")
				.EndCommand();

			builder.AppendLine($"UPDATE [{renameColumnOperation.Schema}].[{renameColumnOperation.Table}]")
				.AppendLine($"SET [{endingState}] = JSON_MODIFY([{endingState}], '$.{newPropertyName}'," +
					$"COALESCE(JSON_VALUE([{endingState}], '$.{oldPropertyName}'), ''));")
				.EndCommand();

			builder.AppendLine($"UPDATE [{renameColumnOperation.Schema}].[{renameColumnOperation.Table}]")
				.AppendLine($"SET [{previousState}] = JSON_MODIFY([{previousState}], 'strict $.{newPropertyName}', NULL)")
				.AppendLine($"WHERE JSON_PATH_EXISTS([{previousState}] , '$.{oldPropertyName}') = 0;")
				.EndCommand();

			builder.AppendLine($"UPDATE [{renameColumnOperation.Schema}].[{renameColumnOperation.Table}]")
				.AppendLine($"SET [{endingState}] = JSON_MODIFY([{endingState}], 'strict $.{newPropertyName}', NULL)")
				.AppendLine($"WHERE JSON_PATH_EXISTS([{endingState}] , '$.{oldPropertyName}') = 0;")
				.EndCommand();

			builder.AppendLine($"UPDATE [{renameColumnOperation.Schema}].[{renameColumnOperation.Table}]")
				.AppendLine($"SET [{previousState}] = JSON_MODIFY([{previousState}], '$.{oldPropertyName}', NULL),")
				.AppendLine($"[{endingState}] = JSON_MODIFY([{endingState}], '$.{oldPropertyName}', NULL);")
				.EndCommand();
		}

		private static void GenerateDrop(
		DropColumnOperation dropColumnOperation,
		MigrationCommandListBuilder builder)
		{
			if (dropColumnOperation.Schema is not AuditableEntityFactory.AUDIT_SHCEMA
				|| !dropColumnOperation.Name.StartsWith(AuditableEntityFactory.PROPERTY_NAME_PREFIX))
			{
				return;
			}

			var propertyName = dropColumnOperation.Name[AuditableEntityFactory.PROPERTY_NAME_PREFIX.Length..];

			var previousState = nameof(AuditingEntity.PreviousState);
			var endingState = nameof(AuditingEntity.EndingState);

			builder.AppendLine($"UPDATE [{dropColumnOperation.Schema}].[{dropColumnOperation.Table}]")
				.AppendLine($"SET [{previousState}] = JSON_MODIFY([{previousState}], '$.{propertyName}', NULL),")
				.AppendLine($"[{endingState}] = JSON_MODIFY([{endingState}], '$.{propertyName}', NULL);")
				.EndCommand();
		}

		private static void GenerateAdd(
		AddColumnOperation addColumnOperation,
		MigrationCommandListBuilder builder)
		{
			if (addColumnOperation.Schema is not AuditableEntityFactory.AUDIT_SHCEMA
				|| !addColumnOperation.Name.StartsWith(AuditableEntityFactory.PROPERTY_NAME_PREFIX))
			{
				return;
			}

			var propertyName = addColumnOperation.Name[AuditableEntityFactory.PROPERTY_NAME_PREFIX.Length..];

			var previousState = nameof(AuditingEntity.PreviousState);
			var endingState = nameof(AuditingEntity.EndingState);

			builder.AppendLine($"UPDATE [{addColumnOperation.Schema}].[{addColumnOperation.Table}]")
				.AppendLine($"SET [{previousState}] = JSON_MODIFY([{previousState}], '$.{propertyName}', ''),")
				.AppendLine($"[{endingState}] = JSON_MODIFY([{endingState}], '$.{propertyName}', '');")
				.EndCommand();

			builder.AppendLine($"UPDATE [{addColumnOperation.Schema}].[{addColumnOperation.Table}]")
				.AppendLine($"SET [{previousState}] = JSON_MODIFY([{previousState}], 'strict $.{propertyName}', null),")
				.AppendLine($"[{endingState}] = JSON_MODIFY([{endingState}], 'strict $.{propertyName}', null);")
				.EndCommand();
		}
	}
}
