using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersTasks.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAuditEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTaskAudit",
                schema: "audit",
                columns: table => new
                {
                    AuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditOperation = table.Column<int>(type: "int", nullable: false),
                    AuditedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    PreviousState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndingState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity_Id = table.Column<int>(type: "int", nullable: false),
                    Entity_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entity_Priority = table.Column<int>(type: "int", nullable: false),
                    Entity_Status = table.Column<int>(type: "int", nullable: false),
                    Entity_IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Entity_CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entity_UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTaskAudit", x => x.AuditId);
                    table.ForeignKey(
                        name: "FK_AppTaskAudit_Users_Entity_UserId",
                        column: x => x.Entity_UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskAudit_Entity_UserId",
                schema: "audit",
                table: "AppTaskAudit",
                column: "Entity_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTaskAudit",
                schema: "audit");
        }
    }
}
