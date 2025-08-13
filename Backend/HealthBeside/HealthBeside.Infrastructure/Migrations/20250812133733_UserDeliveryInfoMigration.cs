using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthBeside.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserDeliveryInfoMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "MarketOrders");

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeliveryInfoId",
                table: "MarketOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserDeliveryInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PostalIndex = table.Column<int>(type: "integer", nullable: false),
                    StreetName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StreetNumber = table.Column<int>(type: "integer", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeliveryInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDeliveryInfos_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketOrders_UserDeliveryInfoId",
                table: "MarketOrders",
                column: "UserDeliveryInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDeliveryInfos_ApplicationUserId",
                table: "UserDeliveryInfos",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketOrders_UserDeliveryInfos_UserDeliveryInfoId",
                table: "MarketOrders",
                column: "UserDeliveryInfoId",
                principalTable: "UserDeliveryInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketOrders_UserDeliveryInfos_UserDeliveryInfoId",
                table: "MarketOrders");

            migrationBuilder.DropTable(
                name: "UserDeliveryInfos");

            migrationBuilder.DropIndex(
                name: "IX_MarketOrders_UserDeliveryInfoId",
                table: "MarketOrders");

            migrationBuilder.DropColumn(
                name: "UserDeliveryInfoId",
                table: "MarketOrders");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "PatientProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "PatientProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "MarketOrders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
