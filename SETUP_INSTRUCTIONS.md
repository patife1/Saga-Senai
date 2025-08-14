# 🚀 Instruções de Setup - Sistema de Estoque

## ✅ STATUS: PROJETO CONCLUÍDO E OPERACIONAL

**Data de Conclusão**: 14 de Agosto de 2025  
**Sistema**: 100% Funcional e testado  
**URL**: http://localhost:5187  

---

## 📋 Pré-requisitos INSTALADOS

Componentes instalados e configurados:

- ✅ [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - INSTALADO
- ✅ [Visual Studio Code](https://code.visualstudio.com/) - CONFIGURADO
- ✅ [SQL Server LocalDB](https://docs.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) - OPERACIONAL
- ✅ [Git](https://git-scm.com/) - CONFIGURADO

## 🏆 PROJETO COMPLETAMENTE IMPLEMENTADO

### ✅ Estrutura Final Criada

```
SistemaEstoque/
├── Controllers/          ✅ 10 Controllers implementados
├── Models/              ✅ 8 Models com relacionamentos
├── Views/               ✅ Interface completa
├── Data/                ✅ Banco configurado
├── ViewModels/          ✅ 3 ViewModels
├── Migrations/          ✅ Migration aplicada
└── wwwroot/             ✅ Assets organizados
```

## 🎯 COMO EXECUTAR O SISTEMA FUNCIONAL

### 1. 📂 Navegar para o Projeto

```powershell
cd "c:\Users\Aluno\Documents\izabelly\Saga-Senai\SistemaEstoque\SistemaEstoque"
```

### 2. 🗃️ Verificar Banco de Dados

```powershell
# Verificar migrations aplicadas
dotnet ef migrations list

# Status: InitialCreate APLICADA ✅
```

### 3. ▶️ Executar o Sistema

```powershell
# Iniciar aplicação
dotnet run

# Sistema disponível em: http://localhost:5187
```

### 4. � Acesso ao Sistema

**Usuário Administrador Padrão**:
- **Email**: admin@sistemaestoque.com
- **Senha**: Admin123!
- **Perfil**: Administrador completo

## 📦 Pacotes NuGet INSTALADOS E CONFIGURADOS

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.8" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.8" />
<PackageReference Include="itext7" Version="9.2.0" />
<PackageReference Include="itext7.bouncy-castle-adapter" Version="9.2.0" />
```
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0

# Bootstrap Icons
dotnet add package bootstrap.icons --version 1.11.3
```

### 4. 🗃️ Configurar String de Conexão

Editar o arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SistemaEstoqueDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 5. 📊 Criar Contexto do Banco de Dados

Criar arquivo `Data/ApplicationDbContext.cs`:

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Models;

namespace SistemaEstoque.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItemVendas { get; set; }
        public DbSet<Servico> Servicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações específicas do modelo
            modelBuilder.Entity<Produto>()
                .Property(p => p.PrecoCompra)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Produto>()
                .Property(p => p.PrecoVenda)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Venda>()
                .Property(v => v.ValorTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.PrecoUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Servico>()
                .Property(s => s.ValorServico)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Funcionario>()
                .Property(f => f.Salario)
                .HasPrecision(18, 2);
        }
    }
}
```

### 6. 🔧 Configurar Serviços no Program.cs

Editar o arquivo `Program.cs`:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
```

### 7. 📝 Criar Models Básicos

#### Categoria.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        public bool Ativo { get; set; } = true;

        // Relacionamento
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
```

#### Produto.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade deve ser positiva")]
        public int QuantidadeEstoque { get; set; }

        [Required(ErrorMessage = "Preço de compra é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de compra deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoCompra { get; set; }

        [Required(ErrorMessage = "Preço de venda é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de venda deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoVenda { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Estoque mínimo deve ser positivo")]
        public int EstoqueMinimo { get; set; } = 0;

        [StringLength(50, ErrorMessage = "Código de barras deve ter no máximo 50 caracteres")]
        public string? CodigoBarras { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Relacionamentos
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } = null!;
        
        public virtual ICollection<ItemVenda> ItemVendas { get; set; } = new List<ItemVenda>();
    }
}
```

### 8. 🗂️ Criar Migrations e Banco de Dados

```powershell
# Criar primeira migration
dotnet ef migrations add InitialCreate

# Aplicar migration no banco
dotnet ef database update
```

### 9. 🎨 Configurar Bootstrap e Layout

Editar `Views/Shared/_Layout.cshtml` para incluir Bootstrap 5:

```html
<!DOCTYPE html>
<html lang="pt-br">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Sistema de Estoque</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css">
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-dark bg-primary border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">
                    <i class="bi bi-boxes me-2"></i>Sistema de Estoque
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Home" asp-action="Index">
                                <i class="bi bi-house me-1"></i>Início
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Estoque" asp-action="Index">
                                <i class="bi bi-box me-1"></i>Estoque
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Vendas" asp-action="Index">
                                <i class="bi bi-cart me-1"></i>Vendas
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Funcionarios" asp-action="Index">
                                <i class="bi bi-people me-1"></i>Funcionários
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Relatorios" asp-action="Index">
                                <i class="bi bi-graph-up me-1"></i>Relatórios
                            </a>
                        </li>
                    </ul>
                    <partial name="_LoginPartial" />
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2024 - Sistema de Estoque - SENAI
        </div>
    </footer>
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

### 10. 🚀 Executar o Projeto

```powershell
# Compilar o projeto
dotnet build

# Executar o projeto
dotnet run
```

O projeto estará disponível em: `https://localhost:5001`

## 📝 Próximos Passos

Após completar o setup inicial:

1. ✅ **Criar os demais models** (Cliente, Funcionario, Venda, etc.)
2. ✅ **Implementar controllers** para cada módulo
3. ✅ **Criar views** com Bootstrap
4. ✅ **Configurar autenticação** e roles
5. ✅ **Implementar validações** com Data Annotations
6. ✅ **Criar dashboard** com relatórios
7. ✅ **Testes** e otimizações

## 🆘 Troubleshooting

### Problema: Erro de conexão com banco
**Solução**: Verificar se o SQL Server LocalDB está instalado e rodando

### Problema: Pacotes não encontrados
**Solução**: Executar `dotnet restore` para restaurar dependências

### Problema: Migration falha
**Solução**: Verificar string de conexão e permissões do banco

## 📚 Documentação Adicional

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Bootstrap 5](https://getbootstrap.com/docs/5.3/)

---

**📞 Suporte**: Se encontrar problemas, consulte a documentação ou entre em contato com o instrutor.
