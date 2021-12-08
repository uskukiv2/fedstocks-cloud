using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace fed.cloud.product.host.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "product");

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GlobalId = table.Column<int>(type: "integer", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "russian")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Name", "GlobalId" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "productcategories",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productcategories_productcategories_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "product",
                        principalTable: "productcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productunits",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rate = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productunits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "counties",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_counties_countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "product",
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_counties_countries_CountryId1",
                        column: x => x.CountryId1,
                        principalSchema: "product",
                        principalTable: "countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: true),
                    GlobalNumber = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    QuantityRate = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId1 = table.Column<int>(type: "integer", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "russian")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Brand", "Name", "GlobalNumber" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_productcategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "product",
                        principalTable: "productcategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_products_productcategories_CategoryId1",
                        column: x => x.CategoryId1,
                        principalSchema: "product",
                        principalTable: "productcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_productunits_UnitId",
                        column: x => x.UnitId,
                        principalSchema: "product",
                        principalTable: "productunits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "sellercompanies",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalName = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "russian")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "OriginalName" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellercompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sellercompanies_counties_CountyId",
                        column: x => x.CountyId,
                        principalSchema: "product",
                        principalTable: "counties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_sellercompanies_countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "product",
                        principalTable: "countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "productsellerprices",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    OriginalCurrencyNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productsellerprices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productsellerprices_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "product",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_productsellerprices_sellercompanies_SellerId",
                        column: x => x.SellerId,
                        principalSchema: "product",
                        principalTable: "sellercompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_counties_CountryId",
                schema: "product",
                table: "counties",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_counties_CountryId1",
                schema: "product",
                table: "counties",
                column: "CountryId1");

            migrationBuilder.CreateIndex(
                name: "IX_countries_SearchVector",
                schema: "product",
                table: "countries",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_productcategories_ParentId",
                schema: "product",
                table: "productcategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryId",
                schema: "product",
                table: "products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryId1",
                schema: "product",
                table: "products",
                column: "CategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_products_SearchVector",
                schema: "product",
                table: "products",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_products_UnitId",
                schema: "product",
                table: "products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_productsellerprices_ProductId",
                schema: "product",
                table: "productsellerprices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_productsellerprices_SellerId",
                schema: "product",
                table: "productsellerprices",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_sellercompanies_CountryId",
                schema: "product",
                table: "sellercompanies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_sellercompanies_CountyId",
                schema: "product",
                table: "sellercompanies",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_sellercompanies_SearchVector",
                schema: "product",
                table: "sellercompanies",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productsellerprices",
                schema: "product");

            migrationBuilder.DropTable(
                name: "products",
                schema: "product");

            migrationBuilder.DropTable(
                name: "sellercompanies",
                schema: "product");

            migrationBuilder.DropTable(
                name: "productcategories",
                schema: "product");

            migrationBuilder.DropTable(
                name: "productunits",
                schema: "product");

            migrationBuilder.DropTable(
                name: "counties",
                schema: "product");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "product");
        }
    }
}
