﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevenNote.Data.Migrations
{
    public partial class AddUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notes_OwnerId",
                table: "Notes",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Users_OwnerId",
                table: "Notes",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Users_OwnerId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_OwnerId",
                table: "Notes");
        }
    }
}
