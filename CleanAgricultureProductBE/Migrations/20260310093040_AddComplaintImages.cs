using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanAgricultureProductBE.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplaintImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplaintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintImage_Complaints_ComplaintId",
                        column: x => x.ComplaintId,
                        principalTable: "Complaints",
                        principalColumn: "ComplaintId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductConplaintImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductComplaintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConplaintImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductConplaintImage_ProductComplaints_ProductComplaintId",
                        column: x => x.ProductComplaintId,
                        principalTable: "ProductComplaints",
                        principalColumn: "ProductComplaintId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintImage_ComplaintId",
                table: "ComplaintImage",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConplaintImage_ProductComplaintId",
                table: "ProductConplaintImage",
                column: "ProductComplaintId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplaintImage");

            migrationBuilder.DropTable(
                name: "ProductConplaintImage");
        }
    }
}
