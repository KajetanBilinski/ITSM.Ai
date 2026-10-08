using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItsmAi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentAuthorType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorType",
                table: "IncidentComments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "SupportAgent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorType",
                table: "IncidentComments");
        }
    }
}
