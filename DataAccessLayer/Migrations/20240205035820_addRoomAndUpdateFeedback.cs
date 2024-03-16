using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class addRoomAndUpdateFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetail_BookingServices_BookingServiceId",
                table: "BookingDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail");

            migrationBuilder.RenameColumn(
                name: "BookingServiceId",
                table: "BookingDetail",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingDetail_BookingServiceId",
                table: "BookingDetail",
                newName: "IX_BookingDetail_RoomId");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "BookingDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail",
                columns: new[] { "ServiceId", "BookingId" });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Rooms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_RoomId",
                table: "Promotions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_RoomId",
                table: "Images",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RoomId",
                table: "Feedbacks",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetail_BookingId",
                table: "BookingDetail",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_UserId",
                table: "Rooms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetail_BookingServices_BookingId",
                table: "BookingDetail",
                column: "BookingId",
                principalTable: "BookingServices",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetail_Rooms_RoomId",
                table: "BookingDetail",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Rooms_RoomId",
                table: "Feedbacks",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Rooms_RoomId",
                table: "Images",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Rooms_RoomId",
                table: "Promotions",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetail_BookingServices_BookingId",
                table: "BookingDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetail_Rooms_RoomId",
                table: "BookingDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Rooms_RoomId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Rooms_RoomId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Rooms_RoomId",
                table: "Promotions");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_RoomId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Images_RoomId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_RoomId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail");

            migrationBuilder.DropIndex(
                name: "IX_BookingDetail_BookingId",
                table: "BookingDetail");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "BookingDetail");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "BookingDetail",
                newName: "BookingServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingDetail_RoomId",
                table: "BookingDetail",
                newName: "IX_BookingDetail_BookingServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail",
                columns: new[] { "ServiceId", "BookingServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetail_BookingServices_BookingServiceId",
                table: "BookingDetail",
                column: "BookingServiceId",
                principalTable: "BookingServices",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
