using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthBeside.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTypeForOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MarketOrders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MarketOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
