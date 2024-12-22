using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNamesToLowerCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_persons_countries_CountryId",
                table: "persons");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "persons",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "PersonName",
                table: "persons",
                newName: "person_name");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "persons",
                newName: "email_address");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "persons",
                newName: "date_of_birth");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "persons",
                newName: "country_id");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "persons",
                newName: "person_id");

            migrationBuilder.RenameIndex(
                name: "IX_persons_CountryId",
                table: "persons",
                newName: "IX_persons_country_id");

            migrationBuilder.RenameColumn(
                name: "CountryName",
                table: "countries",
                newName: "country_name");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "countries",
                newName: "country_id");

            migrationBuilder.AddForeignKey(
                name: "FK_persons_countries_country_id",
                table: "persons",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_persons_countries_country_id",
                table: "persons");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "persons",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "person_name",
                table: "persons",
                newName: "PersonName");

            migrationBuilder.RenameColumn(
                name: "email_address",
                table: "persons",
                newName: "EmailAddress");

            migrationBuilder.RenameColumn(
                name: "date_of_birth",
                table: "persons",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "persons",
                newName: "CountryId");

            migrationBuilder.RenameColumn(
                name: "person_id",
                table: "persons",
                newName: "PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_persons_country_id",
                table: "persons",
                newName: "IX_persons_CountryId");

            migrationBuilder.RenameColumn(
                name: "country_name",
                table: "countries",
                newName: "CountryName");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "countries",
                newName: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_persons_countries_CountryId",
                table: "persons",
                column: "CountryId",
                principalTable: "countries",
                principalColumn: "CountryId");
        }
    }
}
