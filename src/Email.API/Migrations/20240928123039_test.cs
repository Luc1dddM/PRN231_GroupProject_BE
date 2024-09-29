using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Email.API.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "Active", "Body", "Category", "CreatedBy", "CreatedDate", "Description", "EmailTemplateId", "Name", "Subject", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, true, "Hello! Thank you for joining us.", "Welcome", "system", new DateTime(2024, 9, 27, 8, 59, 29, 609, DateTimeKind.Utc).AddTicks(3946), "Email sent to welcome new users.", "TEMPLATE_001", "Welcome Email", "Welcome to Our Service!", null, null },
                    { 2, true, "Click here to reset your password.", "Security", "system", new DateTime(2024, 9, 27, 8, 59, 29, 609, DateTimeKind.Utc).AddTicks(3955), "Email sent to reset user password.", "TEMPLATE_002", "Password Reset", "Reset Your Password", null, null }
                });
        }
    }
}
