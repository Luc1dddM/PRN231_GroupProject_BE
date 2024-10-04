using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Email.API.Migrations
{
    /// <inheritdoc />
    public partial class prayforme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "EmailSends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TemplateType",
                table: "EmailSends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "EmailSends");

            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "EmailSends");
        }
    }
}
