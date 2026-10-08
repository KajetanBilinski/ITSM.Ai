using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItsmAi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequesters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requesters",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false),

                    Name = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false),

                    Email = table.Column<string>(
                        type: "character varying(320)",
                        maxLength: 320,
                        nullable: false),

                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requesters", x => x.Id);
                });

            migrationBuilder.Sql("""
                INSERT INTO "Requesters"
                    ("Id", "Name", "Email", "CreatedAt")
                VALUES
                    (
                        '11111111-1111-1111-1111-111111111111',
                        'Legacy Requester',
                        'legacy@itsmai.local',
                        NOW()
                    );
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "RequesterId",
                table: "Incidents",
                type: "uuid",
                nullable: true);


            migrationBuilder.Sql("""
                UPDATE "Incidents"
                SET "RequesterId" =
                    '11111111-1111-1111-1111-111111111111'
                WHERE "RequesterId" IS NULL;
                """);


            migrationBuilder.AlterColumn<Guid>(
                name: "RequesterId",
                table: "Incidents",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_RequesterId",
                table: "Incidents",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Requesters_Email",
                table: "Requesters",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Requesters_RequesterId",
                table: "Incidents",
                column: "RequesterId",
                principalTable: "Requesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Requesters_RequesterId",
                table: "Incidents");

            migrationBuilder.DropTable(
                name: "Requesters");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_RequesterId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "Incidents");
        }
    }
}
