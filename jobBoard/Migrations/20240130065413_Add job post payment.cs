using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobBoard.Migrations
{
    /// <inheritdoc />
    public partial class Addjobpostpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "JobPostPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckoutSessionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostPayments_JobPosts_JobPostId",
                        column: x => x.JobPostId,
                        principalTable: "JobPosts",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4c6f096f-3d78-4cea-9d5d-fc010db65702"), null, "Business", "BUSINESS" },
                    { new Guid("77d440cf-423c-4d7c-90b4-a9aa9a7eab89"), null, "Admin", "ADMIN" },
                    { new Guid("fb1f3a4b-87e5-49af-b93c-926ab45e9771"), null, "JobSeeker", "JOBSEEKER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostPayments_JobPostId",
                table: "JobPostPayments",
                column: "JobPostId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobPostPayments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4c6f096f-3d78-4cea-9d5d-fc010db65702"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("77d440cf-423c-4d7c-90b4-a9aa9a7eab89"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fb1f3a4b-87e5-49af-b93c-926ab45e9771"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1646b917-2f94-4338-84b6-22d2fe959ea2"), null, "JobSeeker", "JOBSEEKER" },
                    { new Guid("aae11c4d-4319-4ae2-a8a6-070fd2daea5a"), null, "Admin", "ADMIN" },
                    { new Guid("d9ef5714-cf43-4df9-8c13-39a3ae383556"), null, "Business", "BUSINESS" }
                });
        }
    }
}
