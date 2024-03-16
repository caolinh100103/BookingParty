using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deposit",
                columns: table => new
                {
                    DepositId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Percentage = table.Column<float>(type: "real", nullable: false),
                    BookingServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposit", x => x.DepositId);
                    table.ForeignKey(
                        name: "FK_Deposit_BookingServices_BookingServiceId",
                        column: x => x.BookingServiceId,
                        principalTable: "BookingServices",
                        principalColumn: "BookingServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deposit_BookingServiceId",
                table: "Deposit",
                column: "BookingServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deposit");
        }
    }
}
