using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Data
{
    public static class SeedServices
    {
        public static async Task SeedFuncionarios(ApplicationDbContext context)
        {
            var funcionarios = new List<Funcionario>
            {
                new Funcionario
                {
                    Nome = "João Silva",
                    CPF = "12345678901",
                    Email = "joao.silva@sistemaestoque.com",
                    Telefone = "(11) 98765-4321",
                    Cargo = "Gerente",
                    Salario = 5000,
                    DataAdmissao = DateTime.Now.AddYears(-2),
                    Ativo = true
                },
                new Funcionario
                {
                    Nome = "Maria Oliveira",
                    CPF = "98765432100",
                    Email = "maria.oliveira@sistemaestoque.com",
                    Telefone = "(21) 91234-5678",
                    Cargo = "Atendente",
                    Salario = 2500,
                    DataAdmissao = DateTime.Now.AddYears(-1),
                    Ativo = true
                },
                new Funcionario
                {
                    Nome = "Carlos Pereira",
                    CPF = "45678912300",
                    Email = "carlos.pereira@sistemaestoque.com",
                    Telefone = "(31) 99876-5432",
                    Cargo = "Estoquista",
                    Salario = 2000,
                    DataAdmissao = DateTime.Now.AddMonths(-6),
                    Ativo = true
                }
            };

            foreach (var funcionario in funcionarios)
            {
                var exists = await context.Funcionarios.AnyAsync(f => f.CPF == funcionario.CPF);
                if (!exists)
                {
                    context.Funcionarios.Add(funcionario);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
