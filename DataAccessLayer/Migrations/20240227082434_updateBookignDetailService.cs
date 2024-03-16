using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class updateBookignDetailService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "BookingDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BookingDetailId",
                table: "BookingDetail",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail",
                column: "BookingDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetail_ServiceId",
                table: "BookingDetail",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail");

            migrationBuilder.DropIndex(
                name: "IX_BookingDetail_ServiceId",
                table: "BookingDetail");

            migrationBuilder.DropColumn(
                name: "BookingDetailId",
                table: "BookingDetail");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "BookingDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingDetail",
                table: "BookingDetail",
                columns: new[] { "ServiceId", "BookingId" });
        }
    }
}
