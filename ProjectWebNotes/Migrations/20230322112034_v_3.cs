using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectWebNotes.Migrations
{
    /// <inheritdoc />
    public partial class v_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategory_Categories_CategoryID",
                table: "PostCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_PostCategory_Posts_PostID",
                table: "PostCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategory",
                table: "PostCategory");

            migrationBuilder.RenameTable(
                name: "PostCategory",
                newName: "PostCategories");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategory_PostID",
                table: "PostCategories",
                newName: "IX_PostCategories_PostID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories",
                columns: new[] { "CategoryID", "PostID" });

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategories_Categories_CategoryID",
                table: "PostCategories",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategories_Posts_PostID",
                table: "PostCategories",
                column: "PostID",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategories_Categories_CategoryID",
                table: "PostCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostCategories_Posts_PostID",
                table: "PostCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories");

            migrationBuilder.RenameTable(
                name: "PostCategories",
                newName: "PostCategory");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategories_PostID",
                table: "PostCategory",
                newName: "IX_PostCategory_PostID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategory",
                table: "PostCategory",
                columns: new[] { "CategoryID", "PostID" });

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategory_Categories_CategoryID",
                table: "PostCategory",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategory_Posts_PostID",
                table: "PostCategory",
                column: "PostID",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
