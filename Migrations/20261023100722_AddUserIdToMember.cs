using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "members",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_members_user_id",
                table: "members",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_members_users_user_id",
                table: "members",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_members_users_user_id",
                table: "members");

            migrationBuilder.DropIndex(
                name: "ix_members_user_id",
                table: "members");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "members");
        }
    }
}
