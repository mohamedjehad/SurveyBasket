using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuestionIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Questions_QuestionId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Questions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionId",
                table: "Questions",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Questions_QuestionId",
                table: "Questions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
