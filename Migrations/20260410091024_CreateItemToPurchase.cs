using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersonalProject.Migrations
{
    /// <inheritdoc />
    public partial class CreateItemToPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Payment_ShoppingCartId",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_ShoppingCartId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId",
                table: "CartItem");

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalMoney",
                table: "Payment",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "Payment",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ItemToPurchase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ShoppingCartId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemToPurchase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemToPurchase_CartItem_ProductId",
                        column: x => x.ProductId,
                        principalTable: "CartItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemToPurchase_Payment_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Payment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemToPurchase_ProductId",
                table: "ItemToPurchase",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemToPurchase_ShoppingCartId",
                table: "ItemToPurchase",
                column: "ShoppingCartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemToPurchase");

            migrationBuilder.DropColumn(
                name: "SubTotalMoney",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "Payment");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Payment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartId",
                table: "CartItem",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ShoppingCartId",
                table: "CartItem",
                column: "ShoppingCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Payment_ShoppingCartId",
                table: "CartItem",
                column: "ShoppingCartId",
                principalTable: "Payment",
                principalColumn: "Id");
        }
    }
}
