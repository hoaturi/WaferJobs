using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobBoard.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

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
                    Label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    Label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
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
                    Label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Slug = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
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
                    { new Guid("658be231-d50d-46d9-b73a-bad7210d9de7"), null, "JobSeeker", "JOBSEEKER" },
                    { new Guid("7860e1d8-62f3-467d-911a-f5d147aba8ba"), null, "Admin", "ADMIN" },
                    { new Guid("9a27d3fc-5374-4c3b-a0e2-a6ce9d9b10df"), null, "Business", "BUSINESS" }
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
                columns: new[] { "Id", "Label", "Slug" },
                values: new object[,]
                {
                    { 1, "Engineering", "engineering" },
                    { 2, "R&D", "r&d" },
                    { 3, "Q&A", "q&a" },
                    { 4, "Sales", "sales" },
                    { 5, "Marketing", "marketing" },
                    { 6, "Human Resources", "human-resources" },
                    { 7, "Information Technology", "information-technology" },
                    { 8, "Finance", "finance" },
                    { 9, "Customer Support", "customer-support" },
                    { 10, "Product Management", "product-management" },
                    { 11, "Legal", "legal" },
                    { 12, "Technician", "technician" },
                    { 13, "Operations", "operations" },
                    { 14, "Other", "other" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "Label", "Slug" },
                values: new object[,]
                {
                    { 1, "AF", "Afghanistan", "afghanistan" },
                    { 2, "AL", "Albania", "albania" },
                    { 3, "DZ", "Algeria", "algeria" },
                    { 4, "AS", "American Samoa", "american-samoa" },
                    { 5, "AD", "Andorra", "andorra" },
                    { 6, "AO", "Angola", "angola" },
                    { 7, "AI", "Anguilla", "anguilla" },
                    { 8, "AQ", "Antarctica", "antarctica" },
                    { 9, "AG", "Antigua and Barbuda", "antigua-and-barbuda" },
                    { 10, "AR", "Argentina", "argentina" },
                    { 11, "AM", "Armenia", "armenia" },
                    { 12, "AW", "Aruba", "aruba" },
                    { 13, "AP", "Asia/Pacific Region", "asia/pacific-region" },
                    { 14, "AU", "Australia", "australia" },
                    { 15, "AT", "Austria", "austria" },
                    { 16, "AZ", "Azerbaijan", "azerbaijan" },
                    { 17, "BS", "Bahamas", "bahamas" },
                    { 18, "BH", "Bahrain", "bahrain" },
                    { 19, "BD", "Bangladesh", "bangladesh" },
                    { 20, "BB", "Barbados", "barbados" },
                    { 21, "BY", "Belarus", "belarus" },
                    { 22, "BE", "Belgium", "belgium" },
                    { 23, "BZ", "Belize", "belize" },
                    { 24, "BJ", "Benin", "benin" },
                    { 25, "BM", "Bermuda", "bermuda" },
                    { 26, "BT", "Bhutan", "bhutan" },
                    { 27, "BO", "Bolivia", "bolivia" },
                    { 28, "BQ", "Bonaire, Sint Eustatius and Saba", "bonaire,-sint-eustatius-and-saba" },
                    { 29, "BA", "Bosnia and Herzegovina", "bosnia-and-herzegovina" },
                    { 30, "BW", "Botswana", "botswana" },
                    { 31, "BV", "Bouvet Island", "bouvet-island" },
                    { 32, "BR", "Brazil", "brazil" },
                    { 33, "IO", "British Indian Ocean Territory", "british-indian-ocean-territory" },
                    { 34, "BN", "Brunei Darussalam", "brunei-darussalam" },
                    { 35, "BG", "Bulgaria", "bulgaria" },
                    { 36, "BF", "Burkina Faso", "burkina-faso" },
                    { 37, "BI", "Burundi", "burundi" },
                    { 38, "KH", "Cambodia", "cambodia" },
                    { 39, "CM", "Cameroon", "cameroon" },
                    { 40, "CA", "Canada", "canada" },
                    { 41, "CV", "Cape Verde", "cape-verde" },
                    { 42, "KY", "Cayman Islands", "cayman-islands" },
                    { 43, "CF", "Central African Republic", "central-african-republic" },
                    { 44, "TD", "Chad", "chad" },
                    { 45, "CL", "Chile", "chile" },
                    { 46, "CN", "China", "china" },
                    { 47, "CX", "Christmas Island", "christmas-island" },
                    { 48, "CC", "Cocos (Keeling) Islands", "cocos-(keeling)-islands" },
                    { 49, "CO", "Colombia", "colombia" },
                    { 50, "KM", "Comoros", "comoros" },
                    { 51, "CG", "Congo", "congo" },
                    { 52, "CD", "Congo, The Democratic Republic of the", "congo,-the-democratic-republic-of-the" },
                    { 53, "CK", "Cook Islands", "cook-islands" },
                    { 54, "CR", "Costa Rica", "costa-rica" },
                    { 55, "HR", "Croatia", "croatia" },
                    { 56, "CU", "Cuba", "cuba" },
                    { 57, "CW", "Curaçao", "curaçao" },
                    { 58, "CY", "Cyprus", "cyprus" },
                    { 59, "CZ", "Czech Republic", "czech-republic" },
                    { 60, "CI", "Côte d'Ivoire", "côte-d'ivoire" },
                    { 61, "DK", "Denmark", "denmark" },
                    { 62, "DJ", "Djibouti", "djibouti" },
                    { 63, "DM", "Dominica", "dominica" },
                    { 64, "DO", "Dominican Republic", "dominican-republic" },
                    { 65, "EC", "Ecuador", "ecuador" },
                    { 66, "EG", "Egypt", "egypt" },
                    { 67, "SV", "El Salvador", "el-salvador" },
                    { 68, "GQ", "Equatorial Guinea", "equatorial-guinea" },
                    { 69, "ER", "Eritrea", "eritrea" },
                    { 70, "EE", "Estonia", "estonia" },
                    { 71, "ET", "Ethiopia", "ethiopia" },
                    { 72, "FK", "Falkland Islands (Malvinas)", "falkland-islands-(malvinas)" },
                    { 73, "FO", "Faroe Islands", "faroe-islands" },
                    { 74, "FJ", "Fiji", "fiji" },
                    { 75, "FI", "Finland", "finland" },
                    { 76, "FR", "France", "france" },
                    { 77, "GF", "French Guiana", "french-guiana" },
                    { 78, "PF", "French Polynesia", "french-polynesia" },
                    { 79, "TF", "French Southern Territories", "french-southern-territories" },
                    { 80, "GA", "Gabon", "gabon" },
                    { 81, "GM", "Gambia", "gambia" },
                    { 82, "GE", "Georgia", "georgia" },
                    { 83, "DE", "Germany", "germany" },
                    { 84, "GH", "Ghana", "ghana" },
                    { 85, "GI", "Gibraltar", "gibraltar" },
                    { 86, "GR", "Greece", "greece" },
                    { 87, "GL", "Greenland", "greenland" },
                    { 88, "GD", "Grenada", "grenada" },
                    { 89, "GP", "Guadeloupe", "guadeloupe" },
                    { 90, "GU", "Guam", "guam" },
                    { 91, "GT", "Guatemala", "guatemala" },
                    { 92, "GG", "Guernsey", "guernsey" },
                    { 93, "GN", "Guinea", "guinea" },
                    { 94, "GW", "Guinea-Bissau", "guinea-bissau" },
                    { 95, "GY", "Guyana", "guyana" },
                    { 96, "HT", "Haiti", "haiti" },
                    { 97, "HM", "Heard Island and Mcdonald Islands", "heard-island-and-mcdonald-islands" },
                    { 98, "VA", "Holy See (Vatican City State)", "holy-see-(vatican-city-state)" },
                    { 99, "HN", "Honduras", "honduras" },
                    { 100, "HK", "Hong Kong", "hong-kong" },
                    { 101, "HU", "Hungary", "hungary" },
                    { 102, "IS", "Iceland", "iceland" },
                    { 103, "IN", "India", "india" },
                    { 104, "ID", "Indonesia", "indonesia" },
                    { 105, "IR", "Iran, Islamic Republic Of", "iran,-islamic-republic-of" },
                    { 106, "IQ", "Iraq", "iraq" },
                    { 107, "IE", "Ireland", "ireland" },
                    { 108, "IM", "Isle of Man", "isle-of-man" },
                    { 109, "IL", "Israel", "israel" },
                    { 110, "IT", "Italy", "italy" },
                    { 111, "JM", "Jamaica", "jamaica" },
                    { 112, "JP", "Japan", "japan" },
                    { 113, "JE", "Jersey", "jersey" },
                    { 114, "JO", "Jordan", "jordan" },
                    { 115, "KZ", "Kazakhstan", "kazakhstan" },
                    { 116, "KE", "Kenya", "kenya" },
                    { 117, "KI", "Kiribati", "kiribati" },
                    { 118, "KR", "Korea, Republic of", "korea,-republic-of" },
                    { 119, "KW", "Kuwait", "kuwait" },
                    { 120, "KG", "Kyrgyzstan", "kyrgyzstan" },
                    { 121, "LA", "Laos", "laos" },
                    { 122, "LV", "Latvia", "latvia" },
                    { 123, "LB", "Lebanon", "lebanon" },
                    { 124, "LS", "Lesotho", "lesotho" },
                    { 125, "LR", "Liberia", "liberia" },
                    { 126, "LY", "Libyan Arab Jamahiriya", "libyan-arab-jamahiriya" },
                    { 127, "LI", "Liechtenstein", "liechtenstein" },
                    { 128, "LT", "Lithuania", "lithuania" },
                    { 129, "LU", "Luxembourg", "luxembourg" },
                    { 130, "MO", "Macao", "macao" },
                    { 131, "MG", "Madagascar", "madagascar" },
                    { 132, "MW", "Malawi", "malawi" },
                    { 133, "MY", "Malaysia", "malaysia" },
                    { 134, "MV", "Maldives", "maldives" },
                    { 135, "ML", "Mali", "mali" },
                    { 136, "MT", "Malta", "malta" },
                    { 137, "MH", "Marshall Islands", "marshall-islands" },
                    { 138, "MQ", "Martinique", "martinique" },
                    { 139, "MR", "Mauritania", "mauritania" },
                    { 140, "MU", "Mauritius", "mauritius" },
                    { 141, "YT", "Mayotte", "mayotte" },
                    { 142, "MX", "Mexico", "mexico" },
                    { 143, "FM", "Micronesia, Federated States of", "micronesia,-federated-states-of" },
                    { 144, "MD", "Moldova, Republic of", "moldova,-republic-of" },
                    { 145, "MC", "Monaco", "monaco" },
                    { 146, "MN", "Mongolia", "mongolia" },
                    { 147, "ME", "Montenegro", "montenegro" },
                    { 148, "MS", "Montserrat", "montserrat" },
                    { 149, "MA", "Morocco", "morocco" },
                    { 150, "MZ", "Mozambique", "mozambique" },
                    { 151, "MM", "Myanmar", "myanmar" },
                    { 152, "NA", "Namibia", "namibia" },
                    { 153, "NR", "Nauru", "nauru" },
                    { 154, "NP", "Nepal", "nepal" },
                    { 155, "NL", "Netherlands", "netherlands" },
                    { 156, "AN", "Netherlands Antilles", "netherlands-antilles" },
                    { 157, "NC", "New Caledonia", "new-caledonia" },
                    { 158, "NZ", "New Zealand", "new-zealand" },
                    { 159, "NI", "Nicaragua", "nicaragua" },
                    { 160, "NE", "Niger", "niger" },
                    { 161, "NG", "Nigeria", "nigeria" },
                    { 162, "NU", "Niue", "niue" },
                    { 163, "NF", "Norfolk Island", "norfolk-island" },
                    { 164, "KP", "North Korea", "north-korea" },
                    { 165, "MK", "North Macedonia", "north-macedonia" },
                    { 166, "MP", "Northern Mariana Islands", "northern-mariana-islands" },
                    { 167, "NO", "Norway", "norway" },
                    { 168, "OM", "Oman", "oman" },
                    { 169, "PK", "Pakistan", "pakistan" },
                    { 170, "PW", "Palau", "palau" },
                    { 171, "PS", "Palestinian Territory, Occupied", "palestinian-territory,-occupied" },
                    { 172, "PA", "Panama", "panama" },
                    { 173, "PG", "Papua New Guinea", "papua-new-guinea" },
                    { 174, "PY", "Paraguay", "paraguay" },
                    { 175, "PE", "Peru", "peru" },
                    { 176, "PH", "Philippines", "philippines" },
                    { 177, "PN", "Pitcairn Islands", "pitcairn-islands" },
                    { 178, "PL", "Poland", "poland" },
                    { 179, "PT", "Portugal", "portugal" },
                    { 180, "PR", "Puerto Rico", "puerto-rico" },
                    { 181, "QA", "Qatar", "qatar" },
                    { 182, "RE", "Reunion", "reunion" },
                    { 183, "RO", "Romania", "romania" },
                    { 184, "RU", "Russian Federation", "russian-federation" },
                    { 185, "RW", "Rwanda", "rwanda" },
                    { 186, "BL", "Saint Barthélemy", "saint-barthélemy" },
                    { 187, "SH", "Saint Helena", "saint-helena" },
                    { 188, "KN", "Saint Kitts and Nevis", "saint-kitts-and-nevis" },
                    { 189, "LC", "Saint Lucia", "saint-lucia" },
                    { 190, "MF", "Saint Martin", "saint-martin" },
                    { 191, "MF", "Saint Martin", "saint-martin" },
                    { 192, "PM", "Saint Pierre and Miquelon", "saint-pierre-and-miquelon" },
                    { 193, "VC", "Saint Vincent and the Grenadines", "saint-vincent-and-the-grenadines" },
                    { 194, "WS", "Samoa", "samoa" },
                    { 195, "SM", "San Marino", "san-marino" },
                    { 196, "ST", "Sao Tome and Principe", "sao-tome-and-principe" },
                    { 197, "SA", "Saudi Arabia", "saudi-arabia" },
                    { 198, "SN", "Senegal", "senegal" },
                    { 199, "RS", "Serbia", "serbia" },
                    { 200, "CS", "Serbia and Montenegro", "serbia-and-montenegro" },
                    { 201, "SC", "Seychelles", "seychelles" },
                    { 202, "SL", "Sierra Leone", "sierra-leone" },
                    { 203, "SG", "Singapore", "singapore" },
                    { 204, "SX", "Sint Maarten", "sint-maarten" },
                    { 205, "SK", "Slovakia", "slovakia" },
                    { 206, "SI", "Slovenia", "slovenia" },
                    { 207, "SB", "Solomon Islands", "solomon-islands" },
                    { 208, "SO", "Somalia", "somalia" },
                    { 209, "ZA", "South Africa", "south-africa" },
                    { 210, "GS", "South Georgia and the South Sandwich Islands", "south-georgia-and-the-south-sandwich-islands" },
                    { 211, "SS", "South Sudan", "south-sudan" },
                    { 212, "ES", "Spain", "spain" },
                    { 213, "LK", "Sri Lanka", "sri-lanka" },
                    { 214, "SD", "Sudan", "sudan" },
                    { 215, "SR", "Surilabel", "surilabel" },
                    { 216, "SJ", "Svalbard and Jan Mayen", "svalbard-and-jan-mayen" },
                    { 217, "SZ", "Swaziland", "swaziland" },
                    { 218, "SE", "Sweden", "sweden" },
                    { 219, "CH", "Switzerland", "switzerland" },
                    { 220, "SY", "Syrian Arab Republic", "syrian-arab-republic" },
                    { 221, "TW", "Taiwan", "taiwan" },
                    { 222, "TJ", "Tajikistan", "tajikistan" },
                    { 223, "TZ", "Tanzania, United Republic of", "tanzania,-united-republic-of" },
                    { 224, "TH", "Thailand", "thailand" },
                    { 225, "TL", "Timor-Leste", "timor-leste" },
                    { 226, "TG", "Togo", "togo" },
                    { 227, "TK", "Tokelau", "tokelau" },
                    { 228, "TO", "Tonga", "tonga" },
                    { 229, "TT", "Trinidad and Tobago", "trinidad-and-tobago" },
                    { 230, "TN", "Tunisia", "tunisia" },
                    { 231, "TR", "Turkey", "turkey" },
                    { 232, "TM", "Turkmenistan", "turkmenistan" },
                    { 233, "TC", "Turks and Caicos Islands", "turks-and-caicos-islands" },
                    { 234, "TV", "Tuvalu", "tuvalu" },
                    { 235, "UG", "Uganda", "uganda" },
                    { 236, "UA", "Ukraine", "ukraine" },
                    { 237, "AE", "United Arab Emirates", "united-arab-emirates" },
                    { 238, "GB", "United Kingdom", "united-kingdom" },
                    { 239, "US", "United States", "united-states" },
                    { 240, "UM", "United States Minor Outlying Islands", "united-states-minor-outlying-islands" },
                    { 241, "UY", "Uruguay", "uruguay" },
                    { 242, "UZ", "Uzbekistan", "uzbekistan" },
                    { 243, "VU", "Vanuatu", "vanuatu" },
                    { 244, "VE", "Venezuela", "venezuela" },
                    { 245, "VN", "Vietnam", "vietnam" },
                    { 246, "VG", "Virgin Islands, British", "virgin-islands,-british" },
                    { 247, "VI", "Virgin Islands, U.S.", "virgin-islands,-u.s." },
                    { 248, "WF", "Wallis and Futuna", "wallis-and-futuna" },
                    { 249, "EH", "Western Sahara", "western-sahara" },
                    { 250, "YE", "Yemen", "yemen" },
                    { 251, "ZM", "Zambia", "zambia" },
                    { 252, "ZW", "Zimbabwe", "zimbabwe" },
                    { 253, "AX", "Åland Islands", "åland-islands" }
                });

            migrationBuilder.InsertData(
                table: "EmploymentTypes",
                columns: new[] { "Id", "Label", "Slug" },
                values: new object[,]
                {
                    { 1, "Full Time", "full-time" },
                    { 2, "Part Time", "part-time" },
                    { 3, "Contract", "contract" },
                    { 4, "Internship", "internship" },
                    { 5, "Temporary", "temporary" }
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
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Slug",
                table: "Countries",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentTypes_Slug",
                table: "EmploymentTypes",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostPayments_CheckoutSessionId",
                table: "JobPostPayments",
                column: "CheckoutSessionId",
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
                name: "IX_JobPosts_CompanyName",
                table: "JobPosts",
                column: "CompanyName")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_CountryId",
                table: "JobPosts",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_Description",
                table: "JobPosts",
                column: "Description")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_EmploymentTypeId",
                table: "JobPosts",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_Tags",
                table: "JobPosts",
                column: "Tags")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_Title",
                table: "JobPosts",
                column: "Title")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
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
