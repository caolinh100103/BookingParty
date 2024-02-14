using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropOfTransactionHis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "subject",
                table: "TransactionHistories",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "TransactionHistories",
                newName: "BankCode");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "TransactionHistories",
                newName: "TransactionDate");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "TransactionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TransactionHistories");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "TransactionHistories",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "TransactionHistories",
                newName: "subject");

            migrationBuilder.RenameColumn(
                name: "BankCode",
                table: "TransactionHistories",
                newName: "content");
        }
    }
}
