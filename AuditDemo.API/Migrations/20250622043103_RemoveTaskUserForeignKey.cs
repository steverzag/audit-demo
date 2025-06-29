using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersTasks.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTaskUserForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAudit_Users_Entity_UserId",
                schema: "audit",
                table: "AppTaskAudit");

            migrationBuilder.DropIndex(
                name: "IX_AppTaskAudit_Entity_UserId",
                schema: "audit",
                table: "AppTaskAudit");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppTaskAudit_Entity_UserId",
                schema: "audit",
                table: "AppTaskAudit",
                column: "Entity_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAudit_Users_Entity_UserId",
                schema: "audit",
                table: "AppTaskAudit",
                column: "Entity_UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
