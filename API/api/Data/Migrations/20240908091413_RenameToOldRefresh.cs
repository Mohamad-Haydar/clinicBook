using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameToOldRefresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TempRefreshTokenExpiryTime",
                table: "AspNetUsers",
                newName: "OldRefreshTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "TempRefreshToken",
                table: "AspNetUsers",
                newName: "OldRefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OldRefreshTokenExpiryTime",
                table: "AspNetUsers",
                newName: "TempRefreshTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "OldRefreshToken",
                table: "AspNetUsers",
                newName: "TempRefreshToken");
        }
    }
}
