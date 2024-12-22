using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryIdForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("2185c5cd-5b72-444c-9b76-40eee4115338"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("3d362278-f75c-4acb-9845-a5066e4f22a9"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("9238fbc2-fdd4-4aa4-a755-d7023c4be627"));

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[,]
                {
                    { new Guid("043ab8bb-12e1-4a18-b513-5a5075182b52"), "India" },
                    { new Guid("1632f3eb-4e9e-4968-95e1-4c9b733c18d4"), "Canada" },
                    { new Guid("63634744-2da5-4752-b110-c7d75b2f3108"), "United States of America" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryId",
                table: "Persons",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_CountryId",
                table: "Persons",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Countries_CountryId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_CountryId",
                table: "Persons");

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("043ab8bb-12e1-4a18-b513-5a5075182b52"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("1632f3eb-4e9e-4968-95e1-4c9b733c18d4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("63634744-2da5-4752-b110-c7d75b2f3108"));

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[,]
                {
                    { new Guid("2185c5cd-5b72-444c-9b76-40eee4115338"), "Canada" },
                    { new Guid("3d362278-f75c-4acb-9845-a5066e4f22a9"), "United States of America" },
                    { new Guid("9238fbc2-fdd4-4aa4-a755-d7023c4be627"), "India" }
                });
        }
    }
}
