using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobBoard.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessSizeId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StripeCustomerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TwitterUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Businesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Businesses_BusinessSizes_BusinessSizeId",
                        column: x => x.BusinessSizeId,
                        principalTable: "BusinessSizes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    EmploymentTypeId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApplyUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MinSalary = table.Column<int>(type: "integer", nullable: true),
                    MaxSalary = table.Column<int>(type: "integer", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    CompanyLogoUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsRemote = table.Column<bool>(type: "boolean", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPosts_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPosts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPosts_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPosts_EmploymentTypes_EmploymentTypeId",
                        column: x => x.EmploymentTypeId,
                        principalTable: "EmploymentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobPostPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    { new Guid("02c7242b-4cb4-4467-a347-6094510ae46a"), null, "Business", "BUSINESS" },
                    { new Guid("7bebc09a-e88c-4d5f-9a80-7e057f65a79d"), null, "Admin", "ADMIN" },
                    { new Guid("faad2ee8-982f-4ea8-8840-4ece92faac94"), null, "JobSeeker", "JOBSEEKER" }
                });

            migrationBuilder.InsertData(
                table: "BusinessSizes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "1-10" },
                    { 2, "11-50" },
                    { 3, "51-200" },
                    { 4, "201-500" },
                    { 5, "501-1000" },
                    { 6, "1001-5000" },
                    { 7, "5001-10000" },
                    { 8, "10001+" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "Engineering", "engineering" },
                    { 2, "R&D", "research-and-development" },
                    { 3, "Q&A", "quality-assurance" },
                    { 4, "Sales", "sales" },
                    { 5, "Marketing", "marketing" },
                    { 6, "Human Resources", "human-resources" },
                    { 7, "Information Technology", "information-technology" },
                    { 8, "Finance", "finance" },
                    { 9, "Customer Support", "customer-support" },
                    { 10, "Product Management", "product-management" },
                    { 11, "Legal", "legal" },
                    { 12, "Technician", "technicians" },
                    { 13, "Operations", "operations" },
                    { 14, "Other", "other" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "AF", "Afghanistan" },
                    { 2, "AL", "Albania" },
                    { 3, "DZ", "Algeria" },
                    { 4, "AS", "American Samoa" },
                    { 5, "AD", "Andorra" },
                    { 6, "AO", "Angola" },
                    { 7, "AI", "Anguilla" },
                    { 8, "AQ", "Antarctica" },
                    { 9, "AG", "Antigua and Barbuda" },
                    { 10, "AR", "Argentina" },
                    { 11, "AM", "Armenia" },
                    { 12, "AW", "Aruba" },
                    { 13, "AP", "Asia/Pacific Region" },
                    { 14, "AU", "Australia" },
                    { 15, "AT", "Austria" },
                    { 16, "AZ", "Azerbaijan" },
                    { 17, "BS", "Bahamas" },
                    { 18, "BH", "Bahrain" },
                    { 19, "BD", "Bangladesh" },
                    { 20, "BB", "Barbados" },
                    { 21, "BY", "Belarus" },
                    { 22, "BE", "Belgium" },
                    { 23, "BZ", "Belize" },
                    { 24, "BJ", "Benin" },
                    { 25, "BM", "Bermuda" },
                    { 26, "BT", "Bhutan" },
                    { 27, "BO", "Bolivia" },
                    { 28, "BQ", "Bonaire, Sint Eustatius and Saba" },
                    { 29, "BA", "Bosnia and Herzegovina" },
                    { 30, "BW", "Botswana" },
                    { 31, "BV", "Bouvet Island" },
                    { 32, "BR", "Brazil" },
                    { 33, "IO", "British Indian Ocean Territory" },
                    { 34, "BN", "Brunei Darussalam" },
                    { 35, "BG", "Bulgaria" },
                    { 36, "BF", "Burkina Faso" },
                    { 37, "BI", "Burundi" },
                    { 38, "KH", "Cambodia" },
                    { 39, "CM", "Cameroon" },
                    { 40, "CA", "Canada" },
                    { 41, "CV", "Cape Verde" },
                    { 42, "KY", "Cayman Islands" },
                    { 43, "CF", "Central African Republic" },
                    { 44, "TD", "Chad" },
                    { 45, "CL", "Chile" },
                    { 46, "CN", "China" },
                    { 47, "CX", "Christmas Island" },
                    { 48, "CC", "Cocos (Keeling) Islands" },
                    { 49, "CO", "Colombia" },
                    { 50, "KM", "Comoros" },
                    { 51, "CG", "Congo" },
                    { 52, "CD", "Congo, The Democratic Republic of the" },
                    { 53, "CK", "Cook Islands" },
                    { 54, "CR", "Costa Rica" },
                    { 55, "HR", "Croatia" },
                    { 56, "CU", "Cuba" },
                    { 57, "CW", "Curaçao" },
                    { 58, "CY", "Cyprus" },
                    { 59, "CZ", "Czech Republic" },
                    { 60, "CI", "Côte d'Ivoire" },
                    { 61, "DK", "Denmark" },
                    { 62, "DJ", "Djibouti" },
                    { 63, "DM", "Dominica" },
                    { 64, "DO", "Dominican Republic" },
                    { 65, "EC", "Ecuador" },
                    { 66, "EG", "Egypt" },
                    { 67, "SV", "El Salvador" },
                    { 68, "GQ", "Equatorial Guinea" },
                    { 69, "ER", "Eritrea" },
                    { 70, "EE", "Estonia" },
                    { 71, "ET", "Ethiopia" },
                    { 72, "FK", "Falkland Islands (Malvinas)" },
                    { 73, "FO", "Faroe Islands" },
                    { 74, "FJ", "Fiji" },
                    { 75, "FI", "Finland" },
                    { 76, "FR", "France" },
                    { 77, "GF", "French Guiana" },
                    { 78, "PF", "French Polynesia" },
                    { 79, "TF", "French Southern Territories" },
                    { 80, "GA", "Gabon" },
                    { 81, "GM", "Gambia" },
                    { 82, "GE", "Georgia" },
                    { 83, "DE", "Germany" },
                    { 84, "GH", "Ghana" },
                    { 85, "GI", "Gibraltar" },
                    { 86, "GR", "Greece" },
                    { 87, "GL", "Greenland" },
                    { 88, "GD", "Grenada" },
                    { 89, "GP", "Guadeloupe" },
                    { 90, "GU", "Guam" },
                    { 91, "GT", "Guatemala" },
                    { 92, "GG", "Guernsey" },
                    { 93, "GN", "Guinea" },
                    { 94, "GW", "Guinea-Bissau" },
                    { 95, "GY", "Guyana" },
                    { 96, "HT", "Haiti" },
                    { 97, "HM", "Heard Island and Mcdonald Islands" },
                    { 98, "VA", "Holy See (Vatican City State)" },
                    { 99, "HN", "Honduras" },
                    { 100, "HK", "Hong Kong" },
                    { 101, "HU", "Hungary" },
                    { 102, "IS", "Iceland" },
                    { 103, "IN", "India" },
                    { 104, "ID", "Indonesia" },
                    { 105, "IR", "Iran, Islamic Republic Of" },
                    { 106, "IQ", "Iraq" },
                    { 107, "IE", "Ireland" },
                    { 108, "IM", "Isle of Man" },
                    { 109, "IL", "Israel" },
                    { 110, "IT", "Italy" },
                    { 111, "JM", "Jamaica" },
                    { 112, "JP", "Japan" },
                    { 113, "JE", "Jersey" },
                    { 114, "JO", "Jordan" },
                    { 115, "KZ", "Kazakhstan" },
                    { 116, "KE", "Kenya" },
                    { 117, "KI", "Kiribati" },
                    { 118, "KR", "Korea, Republic of" },
                    { 119, "KW", "Kuwait" },
                    { 120, "KG", "Kyrgyzstan" },
                    { 121, "LA", "Laos" },
                    { 122, "LV", "Latvia" },
                    { 123, "LB", "Lebanon" },
                    { 124, "LS", "Lesotho" },
                    { 125, "LR", "Liberia" },
                    { 126, "LY", "Libyan Arab Jamahiriya" },
                    { 127, "LI", "Liechtenstein" },
                    { 128, "LT", "Lithuania" },
                    { 129, "LU", "Luxembourg" },
                    { 130, "MO", "Macao" },
                    { 131, "MG", "Madagascar" },
                    { 132, "MW", "Malawi" },
                    { 133, "MY", "Malaysia" },
                    { 134, "MV", "Maldives" },
                    { 135, "ML", "Mali" },
                    { 136, "MT", "Malta" },
                    { 137, "MH", "Marshall Islands" },
                    { 138, "MQ", "Martinique" },
                    { 139, "MR", "Mauritania" },
                    { 140, "MU", "Mauritius" },
                    { 141, "YT", "Mayotte" },
                    { 142, "MX", "Mexico" },
                    { 143, "FM", "Micronesia, Federated States of" },
                    { 144, "MD", "Moldova, Republic of" },
                    { 145, "MC", "Monaco" },
                    { 146, "MN", "Mongolia" },
                    { 147, "ME", "Montenegro" },
                    { 148, "MS", "Montserrat" },
                    { 149, "MA", "Morocco" },
                    { 150, "MZ", "Mozambique" },
                    { 151, "MM", "Myanmar" },
                    { 152, "NA", "Namibia" },
                    { 153, "NR", "Nauru" },
                    { 154, "NP", "Nepal" },
                    { 155, "NL", "Netherlands" },
                    { 156, "AN", "Netherlands Antilles" },
                    { 157, "NC", "New Caledonia" },
                    { 158, "NZ", "New Zealand" },
                    { 159, "NI", "Nicaragua" },
                    { 160, "NE", "Niger" },
                    { 161, "NG", "Nigeria" },
                    { 162, "NU", "Niue" },
                    { 163, "NF", "Norfolk Island" },
                    { 164, "KP", "North Korea" },
                    { 165, "MK", "North Macedonia" },
                    { 166, "MP", "Northern Mariana Islands" },
                    { 167, "NO", "Norway" },
                    { 168, "OM", "Oman" },
                    { 169, "PK", "Pakistan" },
                    { 170, "PW", "Palau" },
                    { 171, "PS", "Palestinian Territory, Occupied" },
                    { 172, "PA", "Panama" },
                    { 173, "PG", "Papua New Guinea" },
                    { 174, "PY", "Paraguay" },
                    { 175, "PE", "Peru" },
                    { 176, "PH", "Philippines" },
                    { 177, "PN", "Pitcairn Islands" },
                    { 178, "PL", "Poland" },
                    { 179, "PT", "Portugal" },
                    { 180, "PR", "Puerto Rico" },
                    { 181, "QA", "Qatar" },
                    { 182, "RE", "Reunion" },
                    { 183, "RO", "Romania" },
                    { 184, "RU", "Russian Federation" },
                    { 185, "RW", "Rwanda" },
                    { 186, "BL", "Saint Barthélemy" },
                    { 187, "SH", "Saint Helena" },
                    { 188, "KN", "Saint Kitts and Nevis" },
                    { 189, "LC", "Saint Lucia" },
                    { 190, "MF", "Saint Martin" },
                    { 191, "MF", "Saint Martin" },
                    { 192, "PM", "Saint Pierre and Miquelon" },
                    { 193, "VC", "Saint Vincent and the Grenadines" },
                    { 194, "WS", "Samoa" },
                    { 195, "SM", "San Marino" },
                    { 196, "ST", "Sao Tome and Principe" },
                    { 197, "SA", "Saudi Arabia" },
                    { 198, "SN", "Senegal" },
                    { 199, "RS", "Serbia" },
                    { 200, "CS", "Serbia and Montenegro" },
                    { 201, "SC", "Seychelles" },
                    { 202, "SL", "Sierra Leone" },
                    { 203, "SG", "Singapore" },
                    { 204, "SX", "Sint Maarten" },
                    { 205, "SK", "Slovakia" },
                    { 206, "SI", "Slovenia" },
                    { 207, "SB", "Solomon Islands" },
                    { 208, "SO", "Somalia" },
                    { 209, "ZA", "South Africa" },
                    { 210, "GS", "South Georgia and the South Sandwich Islands" },
                    { 211, "SS", "South Sudan" },
                    { 212, "ES", "Spain" },
                    { 213, "LK", "Sri Lanka" },
                    { 214, "SD", "Sudan" },
                    { 215, "SR", "Suriname" },
                    { 216, "SJ", "Svalbard and Jan Mayen" },
                    { 217, "SZ", "Swaziland" },
                    { 218, "SE", "Sweden" },
                    { 219, "CH", "Switzerland" },
                    { 220, "SY", "Syrian Arab Republic" },
                    { 221, "TW", "Taiwan" },
                    { 222, "TJ", "Tajikistan" },
                    { 223, "TZ", "Tanzania, United Republic of" },
                    { 224, "TH", "Thailand" },
                    { 225, "TL", "Timor-Leste" },
                    { 226, "TG", "Togo" },
                    { 227, "TK", "Tokelau" },
                    { 228, "TO", "Tonga" },
                    { 229, "TT", "Trinidad and Tobago" },
                    { 230, "TN", "Tunisia" },
                    { 231, "TR", "Turkey" },
                    { 232, "TM", "Turkmenistan" },
                    { 233, "TC", "Turks and Caicos Islands" },
                    { 234, "TV", "Tuvalu" },
                    { 235, "UG", "Uganda" },
                    { 236, "UA", "Ukraine" },
                    { 237, "AE", "United Arab Emirates" },
                    { 238, "GB", "United Kingdom" },
                    { 239, "US", "United States" },
                    { 240, "UM", "United States Minor Outlying Islands" },
                    { 241, "UY", "Uruguay" },
                    { 242, "UZ", "Uzbekistan" },
                    { 243, "VU", "Vanuatu" },
                    { 244, "VE", "Venezuela" },
                    { 245, "VN", "Vietnam" },
                    { 246, "VG", "Virgin Islands, British" },
                    { 247, "VI", "Virgin Islands, U.S." },
                    { 248, "WF", "Wallis and Futuna" },
                    { 249, "EH", "Western Sahara" },
                    { 250, "YE", "Yemen" },
                    { 251, "ZM", "Zambia" },
                    { 252, "ZW", "Zimbabwe" },
                    { 253, "AX", "Åland Islands" }
                });

            migrationBuilder.InsertData(
                table: "EmploymentTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Full Time" },
                    { 2, "Part Time" },
                    { 3, "Contract" },
                    { 4, "Internship" },
                    { 5, "Temporary" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_BusinessSizeId",
                table: "Businesses",
                column: "BusinessSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_UserId",
                table: "Businesses",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostPayments_JobPostId",
                table: "JobPostPayments",
                column: "JobPostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_BusinessId",
                table: "JobPosts",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_CategoryId",
                table: "JobPosts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_CountryId",
                table: "JobPosts",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_EmploymentTypeId",
                table: "JobPosts",
                column: "EmploymentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "JobPostPayments");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "JobPosts");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "EmploymentTypes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BusinessSizes");
        }
    }
}
