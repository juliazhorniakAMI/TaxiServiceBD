using Microsoft.EntityFrameworkCore.Migrations;

namespace TaxiServiceBD.Migrations
{
    public partial class _spUpdateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedureGetAll =
     @"CREATE PROCEDURE _spUpdateUser @UserID int, @FullName varchar(100),@PhoneNumber varchar(100) AS
    BEGIN
        UPDATE Users
        SET FullName = @FullName,PhoneNumber=@PhoneNumber
        WHERE Id = @UserID;
    END";
            migrationBuilder.Sql(procedureGetAll);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedureGetAll = @"Drop PROCEDURE _spUpdateUser ";
            migrationBuilder.Sql(procedureGetAll);

        }
    }
}
