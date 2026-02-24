using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppCooking.Migrations
{
    /// <inheritdoc />
    public partial class updateRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authorities",
                columns: table => new
                {
                    IdAuthor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    AuthorSpecialization = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthorCountry = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorities", x => x.IdAuthor);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    IdIngredient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IngredientDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    IdCategoryIngredient = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.IdIngredient);
                });

            migrationBuilder.CreateTable(
                name: "RecipeTypes",
                columns: table => new
                {
                    IdRecipeType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeTypes", x => x.IdRecipeType);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    IdRecipe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecipeDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecipeImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRecipeType = table.Column<int>(type: "int", nullable: false),
                    IdAuthor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.IdRecipe);
                    table.ForeignKey(
                        name: "FK_Recipes_Authorities_IdAuthor",
                        column: x => x.IdAuthor,
                        principalTable: "Authorities",
                        principalColumn: "IdAuthor");
                    table.ForeignKey(
                        name: "FK_Recipes_RecipeTypes_IdRecipeType",
                        column: x => x.IdRecipeType,
                        principalTable: "RecipeTypes",
                        principalColumn: "IdRecipeType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    IdRecipeIngredients = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdIngredient = table.Column<int>(type: "int", nullable: false),
                    IdRecipe = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.IdRecipeIngredients);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Ingredients_IdIngredient",
                        column: x => x.IdIngredient,
                        principalTable: "Ingredients",
                        principalColumn: "IdIngredient",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipes_IdRecipe",
                        column: x => x.IdRecipe,
                        principalTable: "Recipes",
                        principalColumn: "IdRecipe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IdIngredient",
                table: "RecipeIngredients",
                column: "IdIngredient");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IdRecipe",
                table: "RecipeIngredients",
                column: "IdRecipe");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IdAuthor",
                table: "Recipes",
                column: "IdAuthor");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IdRecipeType",
                table: "Recipes",
                column: "IdRecipeType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Authorities");

            migrationBuilder.DropTable(
                name: "RecipeTypes");
        }
    }
}
