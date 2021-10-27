using Microsoft.EntityFrameworkCore.Migrations;

namespace TaxiServiceBD.Migrations
{
    public partial class spGetEmployeeById : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Create Procedure spGetUserById
                                 @Id int
                                 as
                                Begin
                                Select * from Users
                                Where Id = @Id
                                End";
                            
            migrationBuilder.Sql(procedure);
          
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Drop procedure spGetUserById";
            migrationBuilder.Sql(procedure);

         
        }
    }
}
