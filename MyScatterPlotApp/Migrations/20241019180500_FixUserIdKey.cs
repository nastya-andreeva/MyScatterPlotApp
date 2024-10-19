using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyScatterPlotApp.Migrations
{
    /// <inheritdoc />
    public partial class FixUserIdKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartDatas_AspNetUsers_UserId",
                table: "ChartDatas");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartData_ApplicationUser_UserId",
                table: "ChartDatas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartData_ApplicationUser_UserId",
                table: "ChartDatas");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartDatas_AspNetUsers_UserId",
                table: "ChartDatas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
