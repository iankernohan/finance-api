using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finance_api.Migrations
{
    /// <inheritdoc />
    public partial class SubCategoryNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SubCategory_SubCategoryId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "transactionType",
                table: "Category",
                newName: "TransactionType");

            migrationBuilder.AlterColumn<int>(
                name: "SubCategoryId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SubCategory_SubCategoryId",
                table: "Transactions",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SubCategory_SubCategoryId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Category",
                newName: "transactionType");

            migrationBuilder.AlterColumn<int>(
                name: "SubCategoryId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SubCategory_SubCategoryId",
                table: "Transactions",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
