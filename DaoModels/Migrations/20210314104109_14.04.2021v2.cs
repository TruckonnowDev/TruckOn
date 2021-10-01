using Microsoft.EntityFrameworkCore.Migrations;

namespace DaoModels.Migrations
{
    public partial class _14042021v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdSubscribe",
                table: "Subscribe_STs",
                newName: "IdSubscribeST");

            migrationBuilder.RenameColumn(
                name: "IdCustomer",
                table: "Subscribe_STs",
                newName: "IdCustomerST");

            migrationBuilder.AddColumn<int>(
                name: "IdCompany",
                table: "Subscribe_STs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCompany",
                table: "Subscribe_STs");

            migrationBuilder.RenameColumn(
                name: "IdSubscribeST",
                table: "Subscribe_STs",
                newName: "IdSubscribe");

            migrationBuilder.RenameColumn(
                name: "IdCustomerST",
                table: "Subscribe_STs",
                newName: "IdCustomer");
        }
    }
}
