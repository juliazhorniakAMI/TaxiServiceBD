using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaxiServiceBD.Migrations
{
    public partial class spCreateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedureGetAll =
    @"CREATE PROCEDURE spCreateUser
    @FullName Varchar(100),
    @PhoneNumber Varchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    Insert into Users(
           [FullName]
           ,[PhoneNumber]
           )
 Values (@FullName, @PhoneNumber)
END
GO";
            migrationBuilder.Sql(procedureGetAll);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedureGetAll = @"Drop PROCEDURE spCreateUser ";
            migrationBuilder.Sql(procedureGetAll);

        }
    }
}
