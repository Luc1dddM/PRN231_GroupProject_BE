using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coupon.Grpc.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    MinAmount = table.Column<double>(type: "float", nullable: true),
                    MaxAmount = table.Column<double>(type: "float", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "CouponCode", "CouponId", "CreatedBy", "CreatedDate", "DiscountAmount", "MaxAmount", "MinAmount", "Quantity", "Status", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "WELCOME10", "81d422f6-9c43-4467-96aa-2d564a19b11a", "Admin", new DateTime(2024, 10, 13, 12, 8, 32, 108, DateTimeKind.Utc).AddTicks(5417), 10.0, 200.0, 50.0, 100, true, null, null },
                    { 2, "SUMMER20", "22816d9c-52b8-4916-a6c7-422110ec8a43", "Admin", new DateTime(2024, 10, 13, 12, 8, 32, 108, DateTimeKind.Utc).AddTicks(5428), 20.0, 500.0, 100.0, 50, true, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
