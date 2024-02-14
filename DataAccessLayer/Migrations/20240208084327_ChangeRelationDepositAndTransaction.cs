using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationDepositAndTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_Bookings_BookingServiceId",
                table: "Deposit");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Users_UserId",
                table: "TransactionHistories");

            migrationBuilder.RenameColumn(
                name: "BookingServiceId",
                table: "Deposit",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposit_BookingServiceId",
                table: "Deposit",
                newName: "IX_Deposit_BookingId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TransactionHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DepositId",
                table: "TransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_DepositId",
                table: "TransactionHistories",
                column: "DepositId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit_Bookings_BookingId",
                table: "Deposit",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Deposit_DepositId",
                table: "TransactionHistories",
                column: "DepositId",
                principalTable: "Deposit",
                principalColumn: "DepositId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Users_UserId",
                table: "TransactionHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_Bookings_BookingId",
                table: "Deposit");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Deposit_DepositId",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Users_UserId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_DepositId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "DepositId",
                table: "TransactionHistories");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Deposit",
                newName: "BookingServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposit_BookingId",
                table: "Deposit",
                newName: "IX_Deposit_BookingServiceId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit_Bookings_BookingServiceId",
                table: "Deposit",
                column: "BookingServiceId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Users_UserId",
                table: "TransactionHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
