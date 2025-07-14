using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthBeside.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurationForMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketOrders",
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
                    table.PrimaryKey("PK_MarketOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketProducts",
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
                    table.PrimaryKey("PK_MarketProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketProducts_MarketCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MarketCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketCartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CartId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketCartItems_MarketCarts_CartId",
                        column: x => x.CartId,
                        principalTable: "MarketCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketCartItems_MarketProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MarketProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketOrderItems_MarketOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "MarketOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketOrderItems_MarketProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MarketProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketReviews_MarketProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MarketProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketCartItems_CartId",
                table: "MarketCartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketCartItems_ProductId",
                table: "MarketCartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketCarts_UserId",
                table: "MarketCarts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketOrderItems_OrderId",
                table: "MarketOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketOrderItems_ProductId",
                table: "MarketOrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketOrders_UserId",
                table: "MarketOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketProducts_CategoryId",
                table: "MarketProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketReviews_ProductId",
                table: "MarketReviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketReviews_UserId",
                table: "MarketReviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketCartItems");

            migrationBuilder.DropTable(
                name: "MarketOrderItems");

            migrationBuilder.DropTable(
                name: "MarketReviews");

            migrationBuilder.DropTable(
                name: "MarketCarts");

            migrationBuilder.DropTable(
                name: "MarketOrders");

            migrationBuilder.DropTable(
                name: "MarketProducts");

            migrationBuilder.DropTable(
                name: "MarketCategories");
        }
    }
}
