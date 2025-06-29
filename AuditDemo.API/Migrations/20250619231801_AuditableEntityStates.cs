using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersTasks.API.Migrations
{
    /// <inheritdoc />
    public partial class AuditableEntityStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndingState",
                schema: "audit",
                table: "UserAudit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousState",
                schema: "audit",
                table: "UserAudit",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndingState",
                schema: "audit",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "PreviousState",
                schema: "audit",
                table: "UserAudit");
        }
    }
}
