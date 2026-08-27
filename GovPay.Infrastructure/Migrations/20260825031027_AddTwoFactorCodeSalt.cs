using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoFactorCodeSalt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwoFactorCodeSalt",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorCodeSalt",
                table: "Users");
        }
    }
}
