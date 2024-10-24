using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderFieldsPaymentValueObjectToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payment_CVV",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_CardName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_CardNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_Expiration",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Payment_PaymentMethod",
                table: "Orders",
                newName: "Payment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payment",
                table: "Orders",
                newName: "Payment_PaymentMethod");

            migrationBuilder.AddColumn<string>(
                name: "Payment_CVV",
                table: "Orders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payment_CardName",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payment_CardNumber",
                table: "Orders",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payment_Expiration",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
