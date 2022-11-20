using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Venues");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 57, 0, 917, DateTimeKind.Local).AddTicks(290));

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 57, 0, 917, DateTimeKind.Local).AddTicks(280));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 57, 0, 917, DateTimeKind.Local).AddTicks(220));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 57, 0, 917, DateTimeKind.Local).AddTicks(250));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 57, 0, 917, DateTimeKind.Local).AddTicks(250));

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 57, 0, 917, DateTimeKind.Local).AddTicks(300));

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 57, 0, 917, DateTimeKind.Local).AddTicks(270));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<long>>(
                name: "AddressId",
                table: "Venues",
                type: "bigint[]",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7600));

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7590));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7510));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7560));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7560));

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "AddressId", "CreatedDate" },
                values: new object[] { new List<long> { 1L }, new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7610) });

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 19, 15, 11, 47, 942, DateTimeKind.Local).AddTicks(7580));
        }
    }
}
