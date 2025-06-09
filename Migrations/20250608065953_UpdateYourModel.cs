using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateYourModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "Skill",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skill_SkillId",
                table: "Skill",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Skill_SkillId",
                table: "Skill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_Skill_SkillId",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_SkillId",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "Skill");
        }
    }
}
