using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Email.API.Migrations
{
    /// <inheritdoc />
    public partial class fff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailSends_EmailTemplates_TemplateId1",
                table: "EmailSends");

            migrationBuilder.DropIndex(
                name: "IX_EmailSends_TemplateId1",
                table: "EmailSends");

            migrationBuilder.DropColumn(
                name: "TemplateId1",
                table: "EmailSends");

            migrationBuilder.AlterColumn<int>(
                name: "TemplateId",
                table: "EmailSends",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailSends_EmailTemplates_TemplateId",
                table: "EmailSends");

            migrationBuilder.DropIndex(
                name: "IX_EmailSends_TemplateId",
                table: "EmailSends");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateId",
                table: "EmailSends",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
