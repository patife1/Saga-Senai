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

            // Criar roles se não existirem
            await CreateRoles(roleManager);

            // Criar usuário administrador padrão
            await CreateAdminUser(userManager, context);

            // Criar funcionário administrador se não existir
            await CreateAdminFuncionario(context);
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
                await context.SaveChangesAsync();
            }
        }
    }
}
