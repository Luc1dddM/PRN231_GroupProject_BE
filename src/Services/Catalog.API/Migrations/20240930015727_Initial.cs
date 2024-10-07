using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "(CONVERT([nvarchar](36),newid()))"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "(CONVERT([nvarchar](36),newid()))"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductCategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "(CONVERT([nvarchar](36),newid()))"),
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updatedby = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.ProductCategoryId);
                    table.ForeignKey(
                        name: "FK_Product_Category_Category",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_Product_Category_Product",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedAt", "CreatedBy", "Name", "Status", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { "3da5cd25-4aa6-41f7-b511-9a79e0a63c1f", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(4911), "Test", "Asus", true, "Brand", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(4914), "Test" },
                    { "7dcaa0ae-4576-42e3-bc82-88bb78393e4a", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(4917), "Test", "Razer", true, "Brand", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(4918), "Test" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CreateBy", "CreateDate", "Description", "ImageUrl", "Name", "Price", "Status", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { "b12da1fc-4bd5-40c3-b445-75b47e6db14b", "Test", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(3857), "Razer Pro Click Humanscale Mouse | Wireless", "Test", "Razer Pro Click Humanscale Mouse | Wireless", 2290000f, true, "Test", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(3868) },
                    { "c63003ba-b30d-4f87-9680-b634cab1317c", "Test", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(3873), "Razer DeathAdder V2 Pro Mouse | Wireless", "Test", "Razer DeathAdder V2 Pro Mouse | Wireless", 1990000f, true, "Test", new DateTime(2024, 9, 30, 8, 57, 25, 502, DateTimeKind.Local).AddTicks(3873) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryId",
                table: "Categories",
                column: "CategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductCategoryId",
                table: "ProductCategories",
                column: "ProductCategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductId",
                table: "ProductCategories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductId",
                table: "Products",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
