using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddGetAllPersonsStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string getAllPersonsSProc = @"
                CREATE OR REPLACE FUNCTION GetAllPersons()
                RETURNS TABLE (PersonId uuid, PersonName VARCHAR, DateOfBirth DATE, Gender VARCHAR, EmailAddress VARCHAR, CountryId uuid
                ) 
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    RETURN QUERY
                    SELECT PersonId, PersonName, DateOfBirth, Gender, EmailAddress, CountryId
                    FROM Persons;
                END;
                $$;   
            ";
            migrationBuilder.Sql(getAllPersonsSProc);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS GetAllPersons();");
        }
    }
}
