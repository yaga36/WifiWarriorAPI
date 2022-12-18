using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class IdentityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AspNetUsers",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

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
                columns: new[] { "ConcurrencyStamp", "Password", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "4057acb2-8b0d-4ab3-944b-5abc8b1269e6", "UserPassword", "UserPasswordHash", "07123456789", "cdfb765c-77fb-4cbc-aed9-becd234a0d8a" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 20, 17, 47, 13, 423, DateTimeKind.Local).AddTicks(3990));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "2a90ec72-c6b3-4185-adc4-c63df8780bba", "UserPassword", null, "68c8c8ee-8ca0-46ad-b7df-4054f34c0a27" });

            migrationBuilder.UpdateData(
                table: "ConnectionInformation",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 20, 17, 47, 13, 423, DateTimeKind.Local).AddTicks(3980));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 20, 17, 47, 13, 423, DateTimeKind.Local).AddTicks(3920));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 20, 17, 47, 13, 423, DateTimeKind.Local).AddTicks(3950));

            migrationBuilder.UpdateData(
                table: "ConnectionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 20, 17, 47, 13, 423, DateTimeKind.Local).AddTicks(3950));

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 20, 17, 47, 13, 423, DateTimeKind.Local).AddTicks(4010));

            migrationBuilder.UpdateData(
                table: "WifiLoginDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedDate",
                value: new DateTime(2022, 11, 20, 17, 47, 13, 423, DateTimeKind.Local).AddTicks(3980));
        }
    }
}
