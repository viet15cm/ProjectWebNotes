using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectWebNotes.Migrations
{
    /// <inheritdoc />
    public partial class v_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreate",
                table: "Posts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreate",
                table: "Posts");
        }
    }
}
