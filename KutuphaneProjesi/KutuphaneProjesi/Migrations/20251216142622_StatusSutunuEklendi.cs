using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutuphaneProjesi.Migrations
{
    /// <inheritdoc />
    public partial class StatusSutunuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Önceki kısıtlamaları kaldır
            migrationBuilder.DropForeignKey(name: "FK_loans_books_BookId", table: "loans");
            migrationBuilder.DropForeignKey(name: "FK_loans_users_UserId", table: "loans");
            migrationBuilder.DropForeignKey(name: "FK_users_roles_RoleId", table: "users");

            // Eski Identity tablolarını temizle
            migrationBuilder.DropTable(name: "AspNetRoleClaims");
            migrationBuilder.DropTable(name: "AspNetUserClaims");
            migrationBuilder.DropTable(name: "AspNetUserLogins");
            migrationBuilder.DropTable(name: "AspNetUserRoles");
            migrationBuilder.DropTable(name: "AspNetUserTokens");
            migrationBuilder.DropTable(name: "roles");
            migrationBuilder.DropTable(name: "AspNetRoles");
            migrationBuilder.DropTable(name: "AspNetUsers");

            // Eski indeksleri temizle
            migrationBuilder.DropIndex(name: "Email", table: "users");
            migrationBuilder.DropIndex(name: "IX_users_RoleId", table: "users");
            migrationBuilder.DropIndex(name: "idx_loan_duedate", table: "loans");
            migrationBuilder.DropIndex(name: "ISBN", table: "books");
            migrationBuilder.DropIndex(name: "idx_book_title", table: "books");
            migrationBuilder.DropIndex(name: "idx_author_name", table: "authors");

            // İsim değişiklikleri
            migrationBuilder.RenameIndex(name: "UserId", table: "loans", newName: "IX_loans_UserId");
            migrationBuilder.RenameIndex(name: "BookId", table: "loans", newName: "IX_loans_BookId");
            migrationBuilder.RenameColumn(name: "ISBN", table: "books", newName: "Isbn");
            migrationBuilder.RenameIndex(name: "CategoryId", table: "books", newName: "IX_books_CategoryId");
            migrationBuilder.RenameIndex(name: "AuthorId", table: "books", newName: "IX_books_AuthorId");

            // --- USERS TABLOSU ---
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            // DÜZELTME 1: CreatedAt -> SQL tarafında varsayılan tarih
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            // --- LOANS TABLOSU ---
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDate",
                table: "loans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            // DÜZELTME 2: LoanDate -> SQL tarafında varsayılan tarih
            migrationBuilder.AlterColumn<DateTime>(
                name: "LoanDate",
                table: "loans",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "FineAmount",
                table: "loans",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true,
                oldDefaultValue: 0m);

            // DÜZELTME 3: DueDate -> Nullable olduğu için default value kaldırıldı
            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "loans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "loans",
                type: "nvarchar(max)",
                nullable: true);

            // --- DİĞER TABLOLAR ---
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "StockQuantity",
                table: "books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "books",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Isbn",
                table: "books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "authors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "authors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            // İlişkileri tekrar oluştur
            migrationBuilder.AddForeignKey(
                name: "FK_loans_books_BookId",
                table: "loans",
                column: "BookId",
                principalTable: "books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loans_users_UserId",
                table: "loans",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down metodu değişmedi, gerekirse burayı da güncelleyebiliriz
            // ama şu anki sorunu Up metodu çözecek.
            migrationBuilder.DropForeignKey(name: "FK_loans_books_BookId", table: "loans");
            migrationBuilder.DropForeignKey(name: "FK_loans_users_UserId", table: "loans");
            migrationBuilder.DropColumn(name: "Status", table: "loans");
            // ... (Geri kalan eski kodlar burada kalabilir)
        }
    }
}