using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalePorductOrderPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrderPrice",
                table: "SaleProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPrice",
                table: "SaleProducts");
        }
    }
}
