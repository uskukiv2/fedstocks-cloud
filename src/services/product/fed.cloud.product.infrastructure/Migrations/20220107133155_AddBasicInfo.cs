using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text;

#nullable disable

namespace fed.cloud.product.infrastructure.Migrations
{
    public partial class AddBasicInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            var countryId = Guid.NewGuid();
            var countyId = Guid.NewGuid();

            //countries
            migrationBuilder.Sql(PrepareSql("product.countries", new[] { "Id", "Name", "GlobalId" }, $"'{countryId}'", "'Russia'",
                "'7'"));

            //counties
            migrationBuilder.Sql(PrepareSql("product.counties", new[] { "Id", "Name", "Number", "CountryId" }, $"'{countyId}'",
                "'Udmurtia'", 18, $"'{countryId}'"));

            //categories
            migrationBuilder.Sql(PrepareSql("product.productcategories", new[] { "Id", "Name" }, 1, "'Bakaley'"));
            migrationBuilder.Sql(PrepareSql("product.productcategories", new[] { "Id", "Name" }, 2, "'Milk'"));
            migrationBuilder.Sql(PrepareSql("product.productcategories", new[] { "Id", "Name" }, 3, "'Frozen'"));
            migrationBuilder.Sql(PrepareSql("product.productcategories", new[] { "Id", "Name", "ParentId" }, 4, "'Bakery products'", 1));

            //units
            migrationBuilder.Sql(PrepareSql("product.productunits", new[] { "Id", "Name", "Rate" }, 1, "'Kilogramms'",
                1));
            migrationBuilder.Sql(PrepareSql("product.productunits", new[] { "Id", "Name", "Rate" }, 2, "'Liters'", 1));
            migrationBuilder.Sql(PrepareSql("product.productunits", new[] { "Id", "Name", "Rate" }, 3, "'Gramms'", 1000));
            migrationBuilder.Sql(PrepareSql("product.productunits", new[] { "Id", "Name", "Rate" }, 4, "'Peace'", 1));

            //sellers
            migrationBuilder.Sql(PrepareSql("product.sellercompanies",
                new[] { "Id", "OriginalName", "CountryId", "CountyId" }, $"'{Guid.NewGuid()}'", "'Pyatorochka'", $"'{countryId}'",
                $"'{countyId}'"));
            migrationBuilder.Sql(PrepareSql("product.sellercompanies",
                new[] { "Id", "OriginalName", "CountryId", "CountyId" }, $"'{Guid.NewGuid()}'", "'Lenta'", $"'{countryId}'", $"'{countyId}'"));
            migrationBuilder.Sql(PrepareSql("product.sellercompanies",
                new[] { "Id", "OriginalName", "CountryId", "CountyId" }, $"'{Guid.NewGuid()}'", "'Magnit'", $"'{countryId}'",
                $"'{countyId}'"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }

        private string PrepareSql(string table, string[] columns, params object[] paramsValues)
        {
            if (columns.Length != paramsValues.Length)
            {
                throw new InvalidOperationException("given inccorect count of columns or values");
            }

            var builder = new StringBuilder();
            builder.Append($"INSERT INTO {table}");
            builder.Append("(");
            for (int i = 0; i < columns.Length; i++)
            {
                builder.Append($@"""{columns[i]}""");
                if (i < columns.Length - 1)
                {
                    builder.Append(", ");
                }
            }
            builder.Append(") ");
            builder.Append("VALUES(");
            for (int i = 0; i < paramsValues.Length; i++)
            {
                builder.Append(paramsValues[i]);
                if (i < paramsValues.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}
