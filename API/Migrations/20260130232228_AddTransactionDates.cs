using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Transactions",
                newName: "Dates_DueDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Transactions",
                newName: "Dates_CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dates_DueDate",
                table: "Transactions",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "Dates_CreatedAt",
                table: "Transactions",
                newName: "CreatedAt");
        }
    }
}
