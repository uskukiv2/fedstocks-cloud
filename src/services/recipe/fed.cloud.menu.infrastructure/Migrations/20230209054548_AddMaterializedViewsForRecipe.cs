using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fed.cloud.menu.infrastructure.Migrations
{
    public partial class AddMaterializedViewsForRecipe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE MATERIALIZED VIEW mview_recipes
                  AS 
                  SELECT r.""id"",
                         r.""name"",
                         r.""ownerid"",
                         r.""tags"",
                         r.""cookingtime"",
                         r.""content""
                  FROM recipes r;");

            migrationBuilder.Sql(
                @"CREATE MATERIALIZED VIEW mview_recipeingredients
                  AS
                  SELECT ri.""id"",
                         i.""name"",
                         i.""productnumber"",
                         ri.""quantity"",
                         u.""rate"",
                         ri.""recipeid""
                  FROM recipeingredients ri
                  INNER JOIN ingredients i ON i.""id"" = ri.""ingredientid""
                  LEFT JOIN units u ON u.""id"" = i.""unitid"";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW mview_recipes");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW mview_recipeingredients");
        }
    }
}
