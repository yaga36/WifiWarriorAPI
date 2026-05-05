using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleConnectionInformationPerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConnectionInformation_ConnectionTypeId",
                table: "ConnectionInformation");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionInformation_ConnectionTypeId",
                table: "ConnectionInformation",
                column: "ConnectionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConnectionInformation_ConnectionTypeId",
                table: "ConnectionInformation");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionInformation_ConnectionTypeId",
                table: "ConnectionInformation",
                column: "ConnectionTypeId",
                unique: true);
        }
    }
}
