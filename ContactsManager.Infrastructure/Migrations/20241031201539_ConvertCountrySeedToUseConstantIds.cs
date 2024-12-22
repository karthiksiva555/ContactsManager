using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCountrySeedToUseConstantIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"Countries\";");
            
            migrationBuilder.InsertData(
                table: "Countries",
                columns: ["CountryId", "CountryName"],
                values: new object[,]
                {
                    { new Guid("3eb16cc8-0528-41eb-a2e4-77b06de04e49"), "United States of America" },
                    { new Guid("a59c1f88-4c90-43e4-9b29-4bb0a104dd13"), "India" },
                    { new Guid("b85f5c5d-234b-49b5-b6f3-afe0b0b6196e"), "Canada" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("3eb16cc8-0528-41eb-a2e4-77b06de04e49"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("a59c1f88-4c90-43e4-9b29-4bb0a104dd13"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("b85f5c5d-234b-49b5-b6f3-afe0b0b6196e"));

            migrationBuilder.InsertData(
                table: "Countries",
                columns: ["CountryId", "CountryName"],
                values: new object[,]
                {
                    { new Guid("377abe11-3abb-4986-a841-070119526276"), "Canada" },
                    { new Guid("7141296d-0f6a-408e-bb53-25ff131ade4b"), "India" },
                    { new Guid("7f65b48d-12cd-450b-8dc5-64cfbafb23ff"), "United States of America" }
                });
        }
    }
}
