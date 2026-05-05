using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class StabilizeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 23, 18, 33, 25, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 12, 19, 21, 2, 251, DateTimeKind.Utc).AddTicks(8380));

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 12, 19, 21, 2, 251, DateTimeKind.Utc).AddTicks(6590));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 12, 19, 21, 2, 251, DateTimeKind.Utc).AddTicks(4060));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 12, 19, 21, 2, 251, DateTimeKind.Utc).AddTicks(4380));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 12, 19, 21, 2, 251, DateTimeKind.Utc).AddTicks(4380));

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 12, 19, 21, 2, 251, DateTimeKind.Utc).AddTicks(7330));

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 12, 19, 21, 2, 251, DateTimeKind.Utc).AddTicks(5730));
        }
    }
}
