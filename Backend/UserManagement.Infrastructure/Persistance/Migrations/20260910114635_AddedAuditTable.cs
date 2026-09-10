using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    actionType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldEntityValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewEntityValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");
        }
    }
}
