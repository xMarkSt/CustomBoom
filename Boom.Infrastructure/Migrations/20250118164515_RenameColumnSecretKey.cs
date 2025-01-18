using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnSecretKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "secret_key",
                table: "players",
                newName: "secretKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "secretKey",
                table: "players",
                newName: "secret_key");
        }
    }
}
