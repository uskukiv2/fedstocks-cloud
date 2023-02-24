using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fed.cloud.menu.infrastructure.Migrations
{
    public partial class UseLowerCaseInNavigationKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_auditlogs_users_UserId",
                table: "auditlogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_units_UnitId",
                table: "ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_recipeingredients_ingredients_IngredientId",
                table: "recipeingredients");

            migrationBuilder.RenameColumn(
                name: "IngredientId",
                table: "recipeingredients",
                newName: "ingredientid");

            migrationBuilder.RenameIndex(
                name: "IX_recipeingredients_IngredientId",
                table: "recipeingredients",
                newName: "IX_recipeingredients_ingredientid");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "ingredients",
                newName: "unitid");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_UnitId",
                table: "ingredients",
                newName: "IX_ingredients_unitid");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "auditlogs",
                newName: "userid");

            migrationBuilder.RenameIndex(
                name: "IX_auditlogs_UserId",
                table: "auditlogs",
                newName: "IX_auditlogs_userid");

            migrationBuilder.AddForeignKey(
                name: "FK_auditlogs_users_userid",
                table: "auditlogs",
                column: "userid",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_units_unitid",
                table: "ingredients",
                column: "unitid",
                principalTable: "units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recipeingredients_ingredients_ingredientid",
                table: "recipeingredients",
                column: "ingredientid",
                principalTable: "ingredients",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_auditlogs_users_userid",
                table: "auditlogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_units_unitid",
                table: "ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_recipeingredients_ingredients_ingredientid",
                table: "recipeingredients");

            migrationBuilder.RenameColumn(
                name: "ingredientid",
                table: "recipeingredients",
                newName: "IngredientId");

            migrationBuilder.RenameIndex(
                name: "IX_recipeingredients_ingredientid",
                table: "recipeingredients",
                newName: "IX_recipeingredients_IngredientId");

            migrationBuilder.RenameColumn(
                name: "unitid",
                table: "ingredients",
                newName: "UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_unitid",
                table: "ingredients",
                newName: "IX_ingredients_UnitId");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "auditlogs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_auditlogs_userid",
                table: "auditlogs",
                newName: "IX_auditlogs_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_auditlogs_users_UserId",
                table: "auditlogs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_units_UnitId",
                table: "ingredients",
                column: "UnitId",
                principalTable: "units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recipeingredients_ingredients_IngredientId",
                table: "recipeingredients",
                column: "IngredientId",
                principalTable: "ingredients",
                principalColumn: "id");
        }
    }
}
