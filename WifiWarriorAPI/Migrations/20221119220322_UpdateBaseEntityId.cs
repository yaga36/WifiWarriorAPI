using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBaseEntityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Venues_VenueId",
                table: "Addresses");

            migrationBuilder.AddColumn<long>(
                name: "BaseEntityId",
                table: "WifiLoginDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BaseEntityId",
                table: "Venues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BaseEntityId",
                table: "ConnectionTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BaseEntityId",
                table: "ConnectionInformation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BaseEntityId",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BaseEntityId", "CreatedDate" },
                values: new object[] { 0L, new DateTime(2022, 11, 19, 22, 3, 21, 901, DateTimeKind.Local).AddTicks(1290) });

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BaseEntityId", "CreatedDate" },
                values: new object[] { 0L, new DateTime(2022, 11, 19, 22, 3, 21, 901, DateTimeKind.Local).AddTicks(1280) });

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BaseEntityId", "CreatedDate" },
                values: new object[] { 0L, new DateTime(2022, 11, 19, 22, 3, 21, 901, DateTimeKind.Local).AddTicks(1230) });

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "BaseEntityId", "CreatedDate" },
                values: new object[] { 0L, new DateTime(2022, 11, 19, 22, 3, 21, 901, DateTimeKind.Local).AddTicks(1250) });

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "BaseEntityId", "CreatedDate" },
                values: new object[] { 0L, new DateTime(2022, 11, 19, 22, 3, 21, 901, DateTimeKind.Local).AddTicks(1260) });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BaseEntityId", "CreatedDate" },
                values: new object[] { 0L, new DateTime(2022, 11, 19, 22, 3, 21, 901, DateTimeKind.Local).AddTicks(1300) });

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BaseEntityId", "CreatedDate" },
                values: new object[] { 0L, new DateTime(2022, 11, 19, 22, 3, 21, 901, DateTimeKind.Local).AddTicks(1270) });

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Venues_VenueId",
                table: "Addresses",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Venues_VenueId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "BaseEntityId",
                table: "WifiLoginDetails");

            migrationBuilder.DropColumn(
                name: "BaseEntityId",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "BaseEntityId",
                table: "ConnectionTypes");

            migrationBuilder.DropColumn(
                name: "BaseEntityId",
                table: "ConnectionInformation");

            migrationBuilder.DropColumn(
                name: "BaseEntityId",
                table: "Addresses");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Venues_VenueId",
                table: "Addresses",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
