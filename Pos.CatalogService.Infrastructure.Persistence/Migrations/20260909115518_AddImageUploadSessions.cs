using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.CatalogService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUploadSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                schema: "catalog",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ImageUploadSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpectedContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageUploadSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageUploadSessions_StorageKey",
                table: "ImageUploadSessions",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageUploadSessions_TenantId_ProductId",
                table: "ImageUploadSessions",
                columns: new[] { "TenantId", "ProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageUploadSessions");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                schema: "catalog",
                table: "ProductImages");
        }
    }
}
