using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Email.API.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Receiver",
                table: "EmailSends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TemplateId1",
                table: "EmailSends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmailSends_TemplateId1",
                table: "EmailSends",
                column: "TemplateId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailSends_EmailTemplates_TemplateId1",
                table: "EmailSends",
                column: "TemplateId1",
                principalTable: "EmailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailSends_EmailTemplates_TemplateId1",
                table: "EmailSends");

            migrationBuilder.DropIndex(
                name: "IX_EmailSends_TemplateId1",
                table: "EmailSends");

            migrationBuilder.DropColumn(
                name: "Receiver",
                table: "EmailSends");

            migrationBuilder.DropColumn(
                name: "TemplateId1",
                table: "EmailSends");
        }
    }
}
