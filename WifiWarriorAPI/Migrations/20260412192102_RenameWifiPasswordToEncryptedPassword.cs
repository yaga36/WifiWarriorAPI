using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiWarriorAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameWifiPasswordToEncryptedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "WifiLoginDetails",
                newName: "EncryptedPassword");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EncryptedPassword",
                table: "WifiLoginDetails",
                newName: "Password");
        }
    }
}
