using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppCooking.Migrations
{
    /// <inheritdoc />
    public partial class updateq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_IdRole",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_users_IdRole",
                table: "Users",
                newName: "IX_Users_IdRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "IdUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "IdRole");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_IdRole",
                table: "Users",
                column: "IdRole",
                principalTable: "Roles",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_IdRole",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "roles");

            migrationBuilder.RenameIndex(
                name: "IX_Users_IdRole",
                table: "users",
                newName: "IX_users_IdRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "IdUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "IdRole");

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_IdRole",
                table: "users",
                column: "IdRole",
                principalTable: "roles",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
