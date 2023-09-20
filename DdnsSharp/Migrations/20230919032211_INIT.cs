using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DdnsSharp.Migrations
{
    /// <inheritdoc />
    public partial class INIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_NetworkConfigs",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Enable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Netinterface = table.Column<string>(type: "TEXT", nullable: true),
                    IpReg = table.Column<byte>(type: "INTEGER", nullable: false),
                    Domains = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NetworkConfigs", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "T_DdnsConfigs",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ServiceName = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<string>(type: "TEXT", nullable: true),
                    Key = table.Column<string>(type: "TEXT", nullable: true),
                    IPV4Guid = table.Column<Guid>(type: "TEXT", nullable: true),
                    IPV6Guid = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DdnsConfigs", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_T_DdnsConfigs_T_NetworkConfigs_IPV4Guid",
                        column: x => x.IPV4Guid,
                        principalTable: "T_NetworkConfigs",
                        principalColumn: "Guid");
                    table.ForeignKey(
                        name: "FK_T_DdnsConfigs_T_NetworkConfigs_IPV6Guid",
                        column: x => x.IPV6Guid,
                        principalTable: "T_NetworkConfigs",
                        principalColumn: "Guid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_DdnsConfigs_IPV4Guid",
                table: "T_DdnsConfigs",
                column: "IPV4Guid");

            migrationBuilder.CreateIndex(
                name: "IX_T_DdnsConfigs_IPV6Guid",
                table: "T_DdnsConfigs",
                column: "IPV6Guid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_DdnsConfigs");

            migrationBuilder.DropTable(
                name: "T_NetworkConfigs");
        }
    }
}
