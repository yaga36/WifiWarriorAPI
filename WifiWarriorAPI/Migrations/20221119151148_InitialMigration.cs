using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddressId = table.Column<List<long>>(type: "bigint[]", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WifiLoginDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ssid = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WifiLoginDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConnectionInformation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConnectionTypeId = table.Column<long>(type: "bigint", nullable: false),
                    WifiLoginDetailsId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectionInformation_ConnectionTypes_ConnectionTypeId",
                        column: x => x.ConnectionTypeId,
                        principalTable: "ConnectionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConnectionInformation_WifiLoginDetails_WifiLoginDetailsId",
                        column: x => x.WifiLoginDetailsId,
                        principalTable: "WifiLoginDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    AddressLine1 = table.Column<string>(type: "text", nullable: false),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<string>(type: "text", nullable: true),
                    County = table.Column<string>(type: "text", nullable: true),
                    Postcode = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    ConnectionInformationId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_ConnectionInformation_ConnectionInformationId",
                        column: x => x.ConnectionInformationId,
                        principalTable: "ConnectionInformation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Addresses_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ConnectionTypes",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Name", "Status", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, 1L, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7510), "Open", 1, null, null },
                    { 2L, 1L, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7560), "Password", 1, null, null },
                    { 3L, 1L, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7560), "Login", 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password", "Username" },
                values: new object[] { 1L, "Test@email.com", "TestName", "TestLastName", "UserPassword", "TestUsername" });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "Id", "AddressId", "CreatedById", "CreatedDate", "Name", "Status", "UpdatedById", "UpdatedDate" },
                values: new object[] { 1L, new List<long> { 1L }, 1L, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7610), "Venue Name", 1, null, null });

            migrationBuilder.InsertData(
                table: "WifiLoginDetails",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Password", "Ssid", "Status", "UpdatedById", "UpdatedDate" },
                values: new object[] { 1L, 1L, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7580), "Password", "SSID", 1, null, null });

            migrationBuilder.InsertData(
                table: "ConnectionInformation",
                columns: new[] { "Id", "ConnectionTypeId", "CreatedById", "CreatedDate", "Status", "UpdatedById", "UpdatedDate", "WifiLoginDetailsId" },
                values: new object[] { 1L, 2L, 1L, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7590), 1, null, null, 1L });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "Area", "ConnectionInformationId", "County", "CreatedById", "CreatedDate", "Latitude", "Longitude", "Postcode", "Status", "UpdatedById", "UpdatedDate", "VenueId" },
                values: new object[] { 1L, "Address Line 1", "Address Line 2", "Area", 1L, "County", 1L, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7600), 0.10000000000000001, 0.10000000000000001, "Postcode", 1, null, null, 1L });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ConnectionInformationId",
                table: "Addresses",
                column: "ConnectionInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_VenueId",
                table: "Addresses",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionInformation_ConnectionTypeId",
                table: "ConnectionInformation",
                column: "ConnectionTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionInformation_WifiLoginDetailsId",
                table: "ConnectionInformation",
                column: "WifiLoginDetailsId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ConnectionInformation");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "ConnectionTypes");

            migrationBuilder.DropTable(
                name: "WifiLoginDetails");
        }
    }
}
