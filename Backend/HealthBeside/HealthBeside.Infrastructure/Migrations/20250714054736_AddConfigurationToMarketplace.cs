using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthBeside.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurationToMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketCart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketCart_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ShippingAddress = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketOrder_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketProduct_MarketCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MarketCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketCartItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CartId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketCartId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketCartItem_MarketCart_MarketCartId",
                        column: x => x.MarketCartId,
                        principalTable: "MarketCart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketCartItem_MarketProduct_MarketProductId",
                        column: x => x.MarketProductId,
                        principalTable: "MarketProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketOrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketOrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketOrderItem_MarketOrder_MarketOrderId",
                        column: x => x.MarketOrderId,
                        principalTable: "MarketOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketOrderItem_MarketProduct_MarketProductId",
                        column: x => x.MarketProductId,
                        principalTable: "MarketProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketReview_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketReview_MarketProduct_MarketProductId",
                        column: x => x.MarketProductId,
                        principalTable: "MarketProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketCart_UserId",
                table: "MarketCart",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketCartItem_MarketCartId",
                table: "MarketCartItem",
                column: "MarketCartId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketCartItem_MarketProductId",
                table: "MarketCartItem",
                column: "MarketProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketOrder_UserId",
                table: "MarketOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketOrderItem_MarketOrderId",
                table: "MarketOrderItem",
                column: "MarketOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketOrderItem_MarketProductId",
                table: "MarketOrderItem",
                column: "MarketProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketProduct_CategoryId",
                table: "MarketProduct",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketReview_MarketProductId",
                table: "MarketReview",
                column: "MarketProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketReview_UserId",
                table: "MarketReview",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketCartItem");

            migrationBuilder.DropTable(
                name: "MarketOrderItem");

            migrationBuilder.DropTable(
                name: "MarketReview");

            migrationBuilder.DropTable(
                name: "MarketCart");

            migrationBuilder.DropTable(
                name: "MarketOrder");

            migrationBuilder.DropTable(
                name: "MarketProduct");

            migrationBuilder.DropTable(
                name: "MarketCategory");
        }
    }
}
