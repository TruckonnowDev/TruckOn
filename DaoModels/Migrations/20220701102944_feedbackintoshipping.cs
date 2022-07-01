using Microsoft.EntityFrameworkCore.Migrations;

namespace DaoModels.Migrations
{
    public partial class feedbackintoshipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Feedbackid",
                table: "Shipping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_Feedbackid",
                table: "Shipping",
                column: "Feedbackid");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipping_Feedbacks_Feedbackid",
                table: "Shipping",
                column: "Feedbackid",
                principalTable: "Feedbacks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipping_Feedbacks_Feedbackid",
                table: "Shipping");

            migrationBuilder.DropIndex(
                name: "IX_Shipping_Feedbackid",
                table: "Shipping");

            migrationBuilder.DropColumn(
                name: "Feedbackid",
                table: "Shipping");
        }
    }
}
