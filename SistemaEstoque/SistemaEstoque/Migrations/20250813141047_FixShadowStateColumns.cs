using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaEstoque.Migrations
{
    /// <inheritdoc />
    public partial class FixShadowStateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Clientes_ClienteId1",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Clientes_ClienteId1",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ClienteId1",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_ClienteId1",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Servicos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId1",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId1",
                table: "Servicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId1",
                table: "Vendas",
                column: "ClienteId1");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_ClienteId1",
                table: "Servicos",
                column: "ClienteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Clientes_ClienteId1",
                table: "Servicos",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Clientes_ClienteId1",
                table: "Vendas",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
