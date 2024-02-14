using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNameOfTableBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetail_BookingServices_BookingId",
                table: "BookingDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Users_UserId",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_BookingServices_BookingServiceId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_BookingServices_BookingServiceId",
                table: "Deposit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingServices",
                table: "BookingServices");

            migrationBuilder.RenameTable(
                name: "BookingServices",
                newName: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_BookingServices_UserId",
                table: "Bookings",
                newName: "IX_Bookings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetail_Bookings_BookingId",
                table: "BookingDetail",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Bookings_BookingServiceId",
                table: "Contracts",
                column: "BookingServiceId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit_Bookings_BookingServiceId",
                table: "Deposit",
                column: "BookingServiceId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetail_Bookings_BookingId",
                table: "BookingDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Bookings_BookingServiceId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_Bookings_BookingServiceId",
                table: "Deposit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "BookingServices");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_UserId",
                table: "BookingServices",
                newName: "IX_BookingServices_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingServices",
                table: "BookingServices",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetail_BookingServices_BookingId",
                table: "BookingDetail",
                column: "BookingId",
                principalTable: "BookingServices",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Users_UserId",
                table: "BookingServices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_BookingServices_BookingServiceId",
                table: "Contracts",
                column: "BookingServiceId",
                principalTable: "BookingServices",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit_BookingServices_BookingServiceId",
                table: "Deposit",
                column: "BookingServiceId",
                principalTable: "BookingServices",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
