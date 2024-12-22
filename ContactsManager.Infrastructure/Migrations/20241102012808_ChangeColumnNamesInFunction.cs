using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNamesInFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string dropFunction = "DROP FUNCTION IF EXISTS GetAllPersons();";
            migrationBuilder.Sql(dropFunction);
            
            string getAllPersonsSProc = @"
                CREATE OR REPLACE FUNCTION GetAllPersons()
                RETURNS TABLE (
                    person_id UUID,
                    person_name VARCHAR(50),
                    date_of_birth TIMESTAMPTZ,
                    gender INT,
                    email_address VARCHAR(50),
                    country_id UUID
                ) AS $$
                BEGIN
                    RETURN QUERY
                    SELECT 
                        p.person_id, 
                        p.person_name, 
                        p.date_of_birth, 
                        p.gender, 
                        p.email_address, 
                        p.country_id
                    FROM 
                        persons p;
                END;
                $$ LANGUAGE plpgsql;  
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
