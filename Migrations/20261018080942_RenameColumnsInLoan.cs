using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_API.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnsInLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_loans_books_bookid",
                table: "loans");

            migrationBuilder.DropForeignKey(
                name: "fk_loans_members_memberid",
                table: "loans");

            migrationBuilder.RenameColumn(
                name: "memberid",
                table: "loans",
                newName: "member_id");

            migrationBuilder.RenameColumn(
                name: "bookid",
                table: "loans",
                newName: "book_id");

            migrationBuilder.RenameIndex(
                name: "ix_loans_memberid",
                table: "loans",
                newName: "ix_loans_member_id");

            migrationBuilder.RenameIndex(
                name: "ix_loans_bookid",
                table: "loans",
                newName: "ix_loans_book_id");

            migrationBuilder.AddForeignKey(
                name: "fk_loans_books_book_id",
                table: "loans",
                column: "book_id",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_loans_members_member_id",
                table: "loans",
                column: "member_id",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_loans_books_book_id",
                table: "loans");

            migrationBuilder.DropForeignKey(
                name: "fk_loans_members_member_id",
                table: "loans");

            migrationBuilder.RenameColumn(
                name: "member_id",
                table: "loans",
                newName: "memberid");

            migrationBuilder.RenameColumn(
                name: "book_id",
                table: "loans",
                newName: "bookid");

            migrationBuilder.RenameIndex(
                name: "ix_loans_member_id",
                table: "loans",
                newName: "ix_loans_memberid");

            migrationBuilder.RenameIndex(
                name: "ix_loans_book_id",
                table: "loans",
                newName: "ix_loans_bookid");

            migrationBuilder.AddForeignKey(
                name: "fk_loans_books_bookid",
                table: "loans",
                column: "bookid",
                principalTable: "books",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_loans_members_memberid",
                table: "loans",
                column: "memberid",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
