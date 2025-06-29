using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersTasks.API.Migrations
{
    /// <inheritdoc />
    public partial class UserAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.CreateTable(
                name: "UserAudit",
                schema: "audit",
                columns: table => new
                {
                    AuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditOperation = table.Column<int>(type: "int", nullable: false),
                    AuditedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Entity_Id = table.Column<int>(type: "int", nullable: false),
                    Entity_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAudit", x => x.AuditId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAudit",
                schema: "audit");
        }
    }
}
