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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.UniqueConstraint("AK_Categories_CategoryId", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.UniqueConstraint("AK_Products_ProductId", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
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
                columns: new[] { "Id", "CategoryId", "CreatedAt", "CreatedBy", "Name", "Status", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "2e167242-59e7-448b-9ebe-893f4c2bc7fb", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(3938), "Test", "Asus", true, "Brand", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(3942), "Test" },
                    { 2, "691ff56c-98fd-45dc-b90d-936dfd9baf56", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(3962), "Test", "Razer", true, "Brand", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(3963), "Test" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "ImageUrl", "Name", "Price", "ProductId", "Status", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { 1, "Test", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(2541), "Razer Pro Click Humanscale Mouse | Wireless", "Test", "Razer Pro Click Humanscale Mouse | Wireless", 2290000f, "b1267079-b139-44b3-99bb-c6d831c8dda7", true, "Test", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(2557) },
                    { 2, "Test", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(2563), "Razer DeathAdder V2 Pro Mouse | Wireless", "Test", "Razer DeathAdder V2 Pro Mouse | Wireless", 1990000f, "73a5e39d-e28e-4753-826a-b7439adf14a2", true, "Test", new DateTime(2024, 10, 9, 7, 16, 23, 905, DateTimeKind.Local).AddTicks(2564) }
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
