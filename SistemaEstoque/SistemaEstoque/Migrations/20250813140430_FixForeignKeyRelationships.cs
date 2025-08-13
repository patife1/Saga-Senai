using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaEstoque.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Clientes_ClienteId",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Funcionarios_FuncionarioId",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Clientes_ClienteId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId1",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_FuncionarioId1",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "FuncionarioId1",
                table: "Vendas");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId1",
                table: "Servicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_ClienteId1",
                table: "Servicos",
                column: "ClienteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Clientes_ClienteId",
                table: "Servicos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Clientes_ClienteId1",
                table: "Servicos",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Funcionarios_FuncionarioId",
                table: "Servicos",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Clientes_ClienteId",
                table: "Vendas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId",
                table: "Vendas",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Clientes_ClienteId",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Clientes_ClienteId1",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Funcionarios_FuncionarioId",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Clientes_ClienteId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_ClienteId1",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Servicos");

            migrationBuilder.AddColumn<int>(
                name: "FuncionarioId1",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_FuncionarioId1",
                table: "Vendas",
                column: "FuncionarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Clientes_ClienteId",
                table: "Servicos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Funcionarios_FuncionarioId",
                table: "Servicos",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Clientes_ClienteId",
                table: "Vendas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId",
                table: "Vendas",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId1",
                table: "Vendas",
                column: "FuncionarioId1",
                principalTable: "Funcionarios",
                principalColumn: "Id");
        }
    }
}
