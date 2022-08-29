using Microsoft.EntityFrameworkCore.Migrations;

namespace DaoModels.Migrations
{
    public partial class feedbackChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Feedbacks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_DriverId",
                table: "Feedbacks",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Drivers_DriverId",
                table: "Feedbacks",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Drivers_DriverId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_DriverId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Feedbacks");
        }
    }
}
