using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InsertPersonStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string createInserPersonSp = @"
                 CREATE OR REPLACE PROCEDURE insert_person(
                    p_person_id UUID,
                    p_person_name VARCHAR(50),
                    p_date_of_birth TIMESTAMPTZ,
                    p_gender INT,
                    p_email_address VARCHAR(50),
                    p_country_id UUID
                )
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    INSERT INTO persons (person_id, person_name, date_of_birth, gender, email_address, country_id)
                    VALUES (p_person_id, p_person_name, p_date_of_birth, p_gender, p_email_address, p_country_id);
                END;
                $$;           
            ";
            migrationBuilder.Sql(createInserPersonSp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP PROCEDURE IF EXISTS insert_person(
                    UUID,
                    VARCHAR(50),
                    TIMESTAMPTZ,
                    INT,
                    VARCHAR(50),
                    UUID
                );
            ");
        }
    }
}
