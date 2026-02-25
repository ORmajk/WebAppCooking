using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppCooking.Migrations
{
    /// <inheritdoc />
    public partial class addlogins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Authorities_IdAuthor",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authorities",
                table: "Authorities");

            migrationBuilder.RenameTable(
                name: "Authorities",
                newName: "Authors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "IdAuthor");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Authors_IdAuthor",
                table: "Recipes",
                column: "IdAuthor",
                principalTable: "Authors",
                principalColumn: "IdAuthor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Authors_IdAuthor",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "Authorities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authorities",
                table: "Authorities",
                column: "IdAuthor");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Authorities_IdAuthor",
                table: "Recipes",
                column: "IdAuthor",
                principalTable: "Authorities",
                principalColumn: "IdAuthor");
        }
    }
}
