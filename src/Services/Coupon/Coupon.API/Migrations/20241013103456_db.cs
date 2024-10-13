using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coupon.API.Migrations
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
                    { 1, "WELCOME20", "1b4ec35c-8cb1-4241-a6e5-5dd96f25f4bb", "Admin", new DateTime(2024, 10, 13, 10, 34, 55, 101, DateTimeKind.Utc).AddTicks(9504), 20.0, 200.0, 50.0, 10, true, null, null },
                    { 2, "SUMMER2024", "78bdadfb-e4e9-4017-8c77-ab26e9a1bb08", "Admin", new DateTime(2024, 10, 13, 10, 34, 55, 101, DateTimeKind.Utc).AddTicks(9514), 50.0, 500.0, 100.0, 5, true, null, null }
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
