using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Email.API.Migrations
{
    /// <inheritdoc />
    public partial class testfinaldb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailSends_EmailTemplates_TemplateId",
                table: "EmailSends");

            migrationBuilder.DropIndex(
                name: "IX_EmailSends_TemplateId",
                table: "EmailSends");

            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "EmailSends");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemplateType",
                table: "EmailSends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSends_TemplateId",
                table: "EmailSends",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailSends_EmailTemplates_TemplateId",
                table: "EmailSends",
                column: "TemplateId",
                principalTable: "EmailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
