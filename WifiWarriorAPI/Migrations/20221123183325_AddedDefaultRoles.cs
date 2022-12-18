using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 153, DateTimeKind.Local).AddTicks(6400));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8e0540d8-37fe-4fcd-ae60-36a21c143448", "7086fafc-2cd1-489a-8979-ae0c29f6fa72", "Administrator", "ADMINISTRATOR" },
                    { "8ef9c8c2-769d-4a06-bc20-fdeca2ffd43d", "f23a5603-9858-4cc8-83c8-12547c8084c8", "User", "USER" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "0268880a-7675-40a1-92cb-f1ba8f020e63", "a9b972c0-d63d-4973-b246-ce83de7c668a" });

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 153, DateTimeKind.Local).AddTicks(6390));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 153, DateTimeKind.Local).AddTicks(6300));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 153, DateTimeKind.Local).AddTicks(6330));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 153, DateTimeKind.Local).AddTicks(6330));

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 153, DateTimeKind.Local).AddTicks(6410));

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 153, DateTimeKind.Local).AddTicks(6380));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e0540d8-37fe-4fcd-ae60-36a21c143448");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ef9c8c2-769d-4a06-bc20-fdeca2ffd43d");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 22, 18, 4, 27, 712, DateTimeKind.Local).AddTicks(8350));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "4057acb2-8b0d-4ab3-944b-5abc8b1269e6", "cdfb765c-77fb-4cbc-aed9-becd234a0d8a" });

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 22, 18, 4, 27, 712, DateTimeKind.Local).AddTicks(8340));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 22, 18, 4, 27, 712, DateTimeKind.Local).AddTicks(8280));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 22, 18, 4, 27, 712, DateTimeKind.Local).AddTicks(8310));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 22, 18, 4, 27, 712, DateTimeKind.Local).AddTicks(8310));

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 22, 18, 4, 27, 712, DateTimeKind.Local).AddTicks(8360));

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 22, 18, 4, 27, 712, DateTimeKind.Local).AddTicks(8330));
        }
    }
}
