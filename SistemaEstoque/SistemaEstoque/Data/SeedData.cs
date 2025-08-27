using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Models;

namespace SistemaEstoque.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Roles e usuário admin
            await CreateRoles(roleManager);
            await CreateAdminUser(userManager, context);

            // Ordem de seeds respeitando dependências
            await SeedCategorias(context);            // base para produtos
            await CreateAdminFuncionario(context);    // garante admin funcionário
            await SeedFuncionarios(context);          // demais funcionários
            await SeedClientes(context);              // clientes para serviços
            await SeedProdutos(context);              // precisa de categorias
            await SeedServicos(context);              // precisa de clientes e funcionários
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Gerente", "Funcionario" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateAdminUser(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            // Dados do administrador padrão
            const string adminEmail = "admin@sistemaestoque.com";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"Usuário administrador criado: {adminEmail} / {adminPassword}");
                }
            }
        }

        private static async Task CreateAdminFuncionario(ApplicationDbContext context)
        {
            const string adminEmail = "admin@sistemaestoque.com";

            // Verificar se já existe um funcionário com este email
            var funcionarioExistente = await context.Funcionarios
                .FirstOrDefaultAsync(f => f.Email == adminEmail);

            if (funcionarioExistente == null)
            {
                var adminFuncionario = new Funcionario
                {
                    Nome = "Administrador do Sistema",
                    CPF = "00000000000",
                    Email = adminEmail,
                    Telefone = "(00) 00000-0000",
                    Cargo = "Administrador",
                    Salario = 0,
                    DataAdmissao = DateTime.Now,
                    Ativo = true
                };

                context.Funcionarios.Add(adminFuncionario);
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedFuncionarios(ApplicationDbContext context)
        {
            var funcionarios = new List<Funcionario>
            {
                new Funcionario { Nome = "João Silva", CPF = "12345678901", Email = "joao.silva@sistemaestoque.com", Telefone = "(11) 98765-4321", Cargo = "Gerente", Salario = 5000, DataAdmissao = DateTime.Now.AddYears(-2), Ativo = true },
                new Funcionario { Nome = "Maria Oliveira", CPF = "98765432100", Email = "maria.oliveira@sistemaestoque.com", Telefone = "(21) 91234-5678", Cargo = "Atendente", Salario = 2500, DataAdmissao = DateTime.Now.AddYears(-1), Ativo = true },
                new Funcionario { Nome = "Carlos Pereira", CPF = "45678912300", Email = "carlos.pereira@sistemaestoque.com", Telefone = "(31) 99876-5432", Cargo = "Estoquista", Salario = 2000, DataAdmissao = DateTime.Now.AddMonths(-6), Ativo = true }
            };

            foreach (var f in funcionarios)
            {
                if (!await context.Funcionarios.AnyAsync(x => x.CPF == f.CPF))
                    context.Funcionarios.Add(f);
            }

            await context.SaveChangesAsync();
        }

        // Seed de Categorias
        private static async Task SeedCategorias(ApplicationDbContext context)
        {
            // Limpar categorias existentes se necessário
            var categoriasExistentes = await context.Categorias.ToListAsync();
            if (categoriasExistentes.Any())
            {
                context.Categorias.RemoveRange(categoriasExistentes);
                await context.SaveChangesAsync();
            }

            var categorias = new List<Categoria>
            {
                new Categoria { Nome = "Chillers", Descricao = "Equipamentos de refrigeração industrial" },
                new Categoria { Nome = "Termorreguladores", Descricao = "Controladores de temperatura industrial" },
                new Categoria { Nome = "Trocadores de Calor", Descricao = "Equipamentos para troca térmica" },
                new Categoria { Nome = "Peças e Componentes", Descricao = "Peças de reposição e componentes" }
            };
            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();
        }

        // Seed de Clientes
        private static async Task SeedClientes(ApplicationDbContext context)
        {
            // Verificar se já existem clientes
            if (await context.Clientes.AnyAsync()) return;

            var clientes = new List<Cliente>
            {
                new Cliente { Nome = "Indústria Plásticos Santos Ltda", CPF = null, Email = "compras@plasticossantos.com.br", Telefone = "(11) 3456-7890", Endereco = "Av. Industrial, 1500 - Santos, SP" },
                new Cliente { Nome = "Hospital São João", CPF = null, Email = "manutencao@hospitalsaojoao.com.br", Telefone = "(11) 2345-6789", Endereco = "Rua da Saúde, 789 - São Paulo, SP" },
                new Cliente { Nome = "Metalúrgica Forte SA", CPF = null, Email = "engenharia@metalurgicaforte.com.br", Telefone = "(11) 4567-8901", Endereco = "Rod. dos Bandeirantes, km 25 - Osasco, SP" },
                new Cliente { Nome = "João Silva - Técnico", CPF = "123.456.789-00", Email = "joao.tecnico@email.com", Telefone = "(11) 99876-5432", Endereco = "Rua das Flores, 123 - Guarulhos, SP" },
                new Cliente { Nome = "Farmácia Central", CPF = null, Email = "contato@farmaciacentral.com.br", Telefone = "(11) 3789-0123", Endereco = "Av. Central, 456 - São Paulo, SP" }
            };
            context.Clientes.AddRange(clientes);
            await context.SaveChangesAsync();
        }

        // Seed de Produtos
        private static async Task SeedProdutos(ApplicationDbContext context)
        {
            // Verificar se já existem produtos
            if (await context.Produtos.AnyAsync()) return;

            var categoriaChillers = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Chillers");
            var categoriaTermoreg = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Termorreguladores");
            var categoriaTrocadores = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Trocadores de Calor");
            var categoriaPecas = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Peças e Componentes");
            
            if (categoriaChillers == null || categoriaTermoreg == null || categoriaTrocadores == null || categoriaPecas == null) return;

            var produtos = new List<Produto>
            {
                // Chillers
                new Produto { Nome = "Chiller para Plástico 10HP", Descricao = "Sistema de refrigeração para indústria plástica, 10HP, condensação a ar", CategoriaId = categoriaChillers.Id, QuantidadeEstoque = 5, EstoqueMinimo = 2, PrecoCompra = 15000m, PrecoVenda = 22500m, CodigoBarras = "CHP10001" },
                new Produto { Nome = "Chiller Hospitalar 15HP", Descricao = "Chiller para equipamentos médicos como tomografia e ressonância, 15HP", CategoriaId = categoriaChillers.Id, QuantidadeEstoque = 3, EstoqueMinimo = 1, PrecoCompra = 25000m, PrecoVenda = 38000m, CodigoBarras = "CHH15001" },
                new Produto { Nome = "Chiller Industrial Metal-Mecânica 20HP", Descricao = "Chiller robusto para indústria metal-mecânica, 20HP, alta eficiência", CategoriaId = categoriaChillers.Id, QuantidadeEstoque = 4, EstoqueMinimo = 1, PrecoCompra = 35000m, PrecoVenda = 52500m, CodigoBarras = "CHI20001" },
                new Produto { Nome = "Unidade Água Gelada 30HP", Descricao = "Unidade de água gelada para grandes instalações industriais, 30HP", CategoriaId = categoriaChillers.Id, QuantidadeEstoque = 2, EstoqueMinimo = 1, PrecoCompra = 55000m, PrecoVenda = 82500m, CodigoBarras = "UAG30001" },
                
                // Termorreguladores
                new Produto { Nome = "Termorregulador 5kW", Descricao = "Termorregulador para controle de temperatura em processos industriais, 5kW", CategoriaId = categoriaTermoreg.Id, QuantidadeEstoque = 8, EstoqueMinimo = 3, PrecoCompra = 8500m, PrecoVenda = 12750m, CodigoBarras = "TR5001" },
                new Produto { Nome = "Termochiller Dupla Função 12kW", Descricao = "Equipamento que atua como chiller e termorregulador, 12kW", CategoriaId = categoriaTermoreg.Id, QuantidadeEstoque = 6, EstoqueMinimo = 2, PrecoCompra = 18000m, PrecoVenda = 27000m, CodigoBarras = "TC12001" },
                
                // Trocadores de Calor
                new Produto { Nome = "Trocador de Calor TC-500", Descricao = "Trocador de calor de placas, capacidade 500kW, inox 316L", CategoriaId = categoriaTrocadores.Id, QuantidadeEstoque = 10, EstoqueMinimo = 4, PrecoCompra = 5200m, PrecoVenda = 7800m, CodigoBarras = "TC500001" },
                new Produto { Nome = "Trocador de Calor TC-1000", Descricao = "Trocador de calor de placas, capacidade 1000kW, alta eficiência", CategoriaId = categoriaTrocadores.Id, QuantidadeEstoque = 7, EstoqueMinimo = 3, PrecoCompra = 8900m, PrecoVenda = 13350m, CodigoBarras = "TC1000001" },
                
                // Peças e Componentes
                new Produto { Nome = "Compressor Copeland 5HP", Descricao = "Compressor scroll Copeland para chillers, 5HP, R-410A", CategoriaId = categoriaPecas.Id, QuantidadeEstoque = 15, EstoqueMinimo = 5, PrecoCompra = 3200m, PrecoVenda = 4800m, CodigoBarras = "CP5001" },
                new Produto { Nome = "Válvula de Expansão Danfoss", Descricao = "Válvula de expansão termostática Danfoss TEN-2", CategoriaId = categoriaPecas.Id, QuantidadeEstoque = 25, EstoqueMinimo = 10, PrecoCompra = 280m, PrecoVenda = 420m, CodigoBarras = "VE001" },
                new Produto { Nome = "Controlador Digital Carel", Descricao = "Controlador digital Carel pCO3 para sistemas de refrigeração", CategoriaId = categoriaPecas.Id, QuantidadeEstoque = 20, EstoqueMinimo = 8, PrecoCompra = 1200m, PrecoVenda = 1800m, CodigoBarras = "CD001" },
                new Produto { Nome = "Condensador a Ar 15kW", Descricao = "Condensador a ar ventilado, capacidade 15kW, tubos de cobre", CategoriaId = categoriaPecas.Id, QuantidadeEstoque = 12, EstoqueMinimo = 4, PrecoCompra = 2800m, PrecoVenda = 4200m, CodigoBarras = "CA15001" }
            };
            context.Produtos.AddRange(produtos);
            await context.SaveChangesAsync();
        }

        // Seed de Serviços
        private static async Task SeedServicos(ApplicationDbContext context)
        {
            // Verificar se já existem serviços
            if (await context.Servicos.AnyAsync()) return;

            var cliente1 = await context.Clientes.FirstOrDefaultAsync(c => c.Email == "compras@plasticossantos.com.br");
            var cliente2 = await context.Clientes.FirstOrDefaultAsync(c => c.Email == "manutencao@hospitalsaojoao.com.br");
            var cliente3 = await context.Clientes.FirstOrDefaultAsync(c => c.Email == "engenharia@metalurgicaforte.com.br");
            var cliente4 = await context.Clientes.FirstOrDefaultAsync(c => c.Email == "joao.tecnico@email.com");
            var funcGerente = await context.Funcionarios.FirstOrDefaultAsync(f => f.Cargo == "Gerente");
            var funcAtendente = await context.Funcionarios.FirstOrDefaultAsync(f => f.Cargo == "Atendente");
            
            if (cliente1 == null || cliente2 == null || cliente3 == null || cliente4 == null || funcGerente == null || funcAtendente == null) return;

            var servicos = new List<Servico>
            {
                new Servico 
                { 
                    ClienteId = cliente1.Id, 
                    FuncionarioId = funcGerente.Id, 
                    TipoServico = "Instalação de Chiller", 
                    Descricao = "Instalação e comissionamento de chiller para plástico 10HP na linha de produção", 
                    DataServico = DateTime.Now.AddDays(-15), 
                    ValorServico = 2500m, 
                    Status = "Concluído", 
                    DataConclusao = DateTime.Now.AddDays(-12),
                    Observacoes = "Instalação realizada com sucesso. Cliente satisfeito com o desempenho."
                },
                new Servico 
                { 
                    ClienteId = cliente2.Id, 
                    FuncionarioId = funcAtendente.Id, 
                    TipoServico = "Manutenção Preventiva", 
                    Descricao = "Manutenção preventiva em chiller hospitalar - limpeza, troca de filtros e verificação geral", 
                    DataServico = DateTime.Now.AddDays(-5), 
                    ValorServico = 1800m, 
                    Status = "Concluído", 
                    DataConclusao = DateTime.Now.AddDays(-4),
                    Observacoes = "Todos os componentes verificados. Próxima manutenção em 6 meses."
                },
                new Servico 
                { 
                    ClienteId = cliente3.Id, 
                    FuncionarioId = funcGerente.Id, 
                    TipoServico = "Reparo de Emergência", 
                    Descricao = "Reparo urgente em chiller industrial - substituição de compressor defeituoso", 
                    DataServico = DateTime.Now.AddDays(-2), 
                    ValorServico = 4500m, 
                    Status = "Em Andamento",
                    Observacoes = "Compressor foi substituído. Aguardando teste de 48h antes da finalização."
                },
                new Servico 
                { 
                    ClienteId = cliente4.Id, 
                    FuncionarioId = funcAtendente.Id, 
                    TipoServico = "Consultoria Técnica", 
                    Descricao = "Consultoria para dimensionamento de sistema de refrigeração para nova instalação", 
                    DataServico = DateTime.Now.AddDays(3), 
                    ValorServico = 800m, 
                    Status = "Agendado",
                    Observacoes = "Visita agendada para análise do local e levantamento de necessidades."
                },
                new Servico 
                { 
                    ClienteId = cliente1.Id, 
                    FuncionarioId = funcGerente.Id, 
                    TipoServico = "Retrofit de Sistema", 
                    Descricao = "Modernização de sistema de refrigeração antigo com novos controladores digitais", 
                    DataServico = DateTime.Now.AddDays(7), 
                    ValorServico = 6200m, 
                    Status = "Agendado",
                    Observacoes = "Projeto aprovado pelo cliente. Equipamentos já encomendados."
                }
            };
            context.Servicos.AddRange(servicos);
            await context.SaveChangesAsync();
        }
    }
}