using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Topup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adddatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargeRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    TerminalId = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RetryCount = table.Column<short>(type: "smallint", nullable: false),
                    ExternalServiceResponse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeRequest", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeRequest");
        }
    }
}
