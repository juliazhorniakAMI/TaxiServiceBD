using Microsoft.EntityFrameworkCore.Migrations;

namespace TaxiServiceBD.Migrations
{
    public partial class spGetUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedureGetAll = @"CREATE PROCEDURE spGetUsers AS BEGIN SELECT * FROM Users;END";
            migrationBuilder.Sql(procedureGetAll);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedureGetAll = @"Drop PROCEDURE spGetUsers ";
            migrationBuilder.Sql(procedureGetAll);
        }
    }
}
