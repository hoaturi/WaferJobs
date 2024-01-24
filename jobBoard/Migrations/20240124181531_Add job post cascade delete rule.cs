using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobBoard.Migrations
{
    /// <inheritdoc />
    public partial class Addjobpostcascadedeleterule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_Businesses_BusinessId",
                table: "JobPosts");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4552f46c-3b1b-49d6-8c9b-6923a3ab4092"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d11eab8-ea2f-4634-9cea-e5244fb73522"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f61c1dd3-508e-4f39-859d-165bdc413010"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1646b917-2f94-4338-84b6-22d2fe959ea2"), null, "JobSeeker", "JOBSEEKER" },
                    { new Guid("aae11c4d-4319-4ae2-a8a6-070fd2daea5a"), null, "Admin", "ADMIN" },
                    { new Guid("d9ef5714-cf43-4df9-8c13-39a3ae383556"), null, "Business", "BUSINESS" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_Businesses_BusinessId",
                table: "JobPosts",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_Businesses_BusinessId",
                table: "JobPosts");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1646b917-2f94-4338-84b6-22d2fe959ea2"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("aae11c4d-4319-4ae2-a8a6-070fd2daea5a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d9ef5714-cf43-4df9-8c13-39a3ae383556"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4552f46c-3b1b-49d6-8c9b-6923a3ab4092"), null, "Business", "BUSINESS" },
                    { new Guid("8d11eab8-ea2f-4634-9cea-e5244fb73522"), null, "Admin", "ADMIN" },
                    { new Guid("f61c1dd3-508e-4f39-859d-165bdc413010"), null, "JobSeeker", "JOBSEEKER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_Businesses_BusinessId",
                table: "JobPosts",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");
        }
    }
}
