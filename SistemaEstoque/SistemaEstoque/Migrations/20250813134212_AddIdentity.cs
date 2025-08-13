using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaEstoque.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_CPF",
                table: "Funcionarios");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_Email",
                table: "Funcionarios");

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId1",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FuncionarioId1",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId1",
                table: "ItemVendas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId1",
                table: "Vendas",
                column: "ClienteId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_FuncionarioId1",
                table: "Vendas",
                column: "FuncionarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVendas_ProdutoId1",
                table: "ItemVendas",
                column: "ProdutoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId1",
                table: "ItemVendas",
                column: "ProdutoId1",
                principalTable: "Produtos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Clientes_ClienteId1",
                table: "Vendas",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId1",
                table: "Vendas",
                column: "FuncionarioId1",
                principalTable: "Funcionarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId1",
                table: "ItemVendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Clientes_ClienteId1",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Funcionarios_FuncionarioId1",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ClienteId1",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_FuncionarioId1",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_ItemVendas_ProdutoId1",
                table: "ItemVendas");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "FuncionarioId1",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ProdutoId1",
                table: "ItemVendas");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Ativo", "DataCadastro", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 8, 13, 9, 43, 11, 220, DateTimeKind.Local).AddTicks(6439), "Componentes para refrigeradores", "Peças de Geladeira" },
                    { 2, true, new DateTime(2025, 8, 13, 9, 43, 11, 220, DateTimeKind.Local).AddTicks(6441), "Componentes para fogões", "Peças de Fogão" },
                    { 3, true, new DateTime(2025, 8, 13, 9, 43, 11, 220, DateTimeKind.Local).AddTicks(6442), "Componentes para microondas", "Peças de Microondas" },
                    { 4, true, new DateTime(2025, 8, 13, 9, 43, 11, 220, DateTimeKind.Local).AddTicks(6443), "Ferramentas para manutenção", "Ferramentas" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_CPF",
                table: "Funcionarios",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_Email",
                table: "Funcionarios",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemVendas_Produtos_ProdutoId",
                table: "ItemVendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
