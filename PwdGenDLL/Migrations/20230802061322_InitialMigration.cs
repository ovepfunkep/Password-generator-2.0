using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PwdGenDLL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Encryptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Link = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encryptions", x => x.Id);
                    table.CheckConstraint("CHK_Encryption_Name_MaxLength", "LENGTH(Name) <= 100");
                });

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keys", x => x.Id);
                    table.CheckConstraint("CHK_Key_Value_MaxLength", "LENGTH(Value) <= 100");
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EncryptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    KeyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Encryptions_EncryptionId",
                        column: x => x.EncryptionId,
                        principalTable: "Encryptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Settings_Keys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "Keys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SourceText = table.Column<string>(type: "TEXT", nullable: true),
                    EncryptedText = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SettingsId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHistories", x => x.Id);
                    table.CheckConstraint("CHK_PasswordHistory_SourceText_MaxLength", "LENGTH(SourceText) <= 100");
                    table.ForeignKey(
                        name: "FK_PasswordHistories_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Encryptions_Name",
                table: "Encryptions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keys_Value",
                table: "Keys",
                column: "Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistories_SettingsId",
                table: "PasswordHistories",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_EncryptionId_KeyId",
                table: "Settings",
                columns: new[] { "EncryptionId", "KeyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_KeyId",
                table: "Settings",
                column: "KeyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordHistories");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Encryptions");

            migrationBuilder.DropTable(
                name: "Keys");
        }
    }
}
