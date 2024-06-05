using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobBoard.Migrations
{
    /// <inheritdoc />
    public partial class EditBusinessEntityFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("29764e9a-bb92-4112-99b6-92a90956af25"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7cf19227-d1d2-43b4-ab87-1fc45c03d93e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("871a23c2-8ef8-44e4-ac15-16b1d72ba809"));

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Businesses",
                newName: "WebsiteUrl");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("71eb2644-23a2-4b6b-beb0-519cbca2cdf9"), null, "JobSeeker", "JOBSEEKER" },
                    { new Guid("81146792-4235-4080-a186-0913966acccf"), null, "Business", "BUSINESS" },
                    { new Guid("bd5b33d8-7e22-415c-ba12-4410567aa48b"), null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("71eb2644-23a2-4b6b-beb0-519cbca2cdf9"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("81146792-4235-4080-a186-0913966acccf"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bd5b33d8-7e22-415c-ba12-4410567aa48b"));

            migrationBuilder.RenameColumn(
                name: "WebsiteUrl",
                table: "Businesses",
                newName: "Url");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("29764e9a-bb92-4112-99b6-92a90956af25"), null, "Admin", "ADMIN" },
                    { new Guid("7cf19227-d1d2-43b4-ab87-1fc45c03d93e"), null, "JobSeeker", "JOBSEEKER" },
                    { new Guid("871a23c2-8ef8-44e4-ac15-16b1d72ba809"), null, "Business", "BUSINESS" }
                });
        }
    }
}
