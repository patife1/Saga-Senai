# 📊 Models Completos - Sistema de Estoque

## 📁 Estrutura dos Models

### 1. Categoria.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome da Categoria")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Relacionamento
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
```

### 2. Produto.cs
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
        [Display(Name = "Nome do Produto")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade deve ser positiva")]
        [Display(Name = "Quantidade em Estoque")]
        public int QuantidadeEstoque { get; set; }

        [Required(ErrorMessage = "Preço de compra é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de compra deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Compra")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoCompra { get; set; }

        [Required(ErrorMessage = "Preço de venda é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de venda deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Venda")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoVenda { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Estoque mínimo deve ser positivo")]
        [Display(Name = "Estoque Mínimo")]
        public int EstoqueMinimo { get; set; } = 0;

        [StringLength(50, ErrorMessage = "Código de barras deve ter no máximo 50 caracteres")]
        [Display(Name = "Código de Barras")]
        public string? CodigoBarras { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Margem de Lucro")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public decimal MargemLucro => PrecoCompra > 0 ? ((PrecoVenda - PrecoCompra) / PrecoCompra) : 0;

        [NotMapped]
        [Display(Name = "Estoque Baixo")]
        public bool EstoqueBaixo => QuantidadeEstoque <= EstoqueMinimo;

        [NotMapped]
        [Display(Name = "Valor Total Estoque")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ValorTotalEstoque => QuantidadeEstoque * PrecoVenda;

        // Relacionamentos
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } = null!;
        
        public virtual ICollection<ItemVenda> ItemVendas { get; set; } = new List<ItemVenda>();
    }
}
```

### 3. Cliente.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        [Display(Name = "CPF")]
        public string? CPF { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [StringLength(200, ErrorMessage = "Endereço deve ter no máximo 200 caracteres")]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        // Relacionamentos
        public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}
```

### 4. Funcionario.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Funcionario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        [Display(Name = "CPF")]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Cargo é obrigatório")]
        [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Salário deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Salário")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? Salario { get; set; }

        [Required(ErrorMessage = "Data de admissão é obrigatória")]
        [Display(Name = "Data de Admissão")]
        [DataType(DataType.Date)]
        public DateTime DataAdmissao { get; set; } = DateTime.Now;

        [Display(Name = "Data de Demissão")]
        [DataType(DataType.Date)]
        public DateTime? DataDemissao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Usuário do Sistema")]
        public string? UserId { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Tempo de Empresa")]
        public string TempoEmpresa
        {
            get
            {
                var fim = DataDemissao ?? DateTime.Now;
                var tempo = fim - DataAdmissao;
                var anos = tempo.Days / 365;
                var meses = (tempo.Days % 365) / 30;
                return $"{anos} anos e {meses} meses";
            }
        }

        // Relacionamentos
        public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}
```

### 5. Venda.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Venda
    {
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "Data da venda é obrigatória")]
        [Display(Name = "Data da Venda")]
        [DataType(DataType.DateTime)]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ValorTotal { get; set; }

        [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
        [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(30, ErrorMessage = "Status deve ter no máximo 30 caracteres")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Finalizada";

        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Quantidade de Itens")]
        public int QuantidadeItens => ItemVendas?.Sum(i => i.Quantidade) ?? 0;

        [NotMapped]
        [Display(Name = "Lucro Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LucroTotal => ItemVendas?.Sum(i => i.LucroItem) ?? 0;

        // Relacionamentos
        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("FuncionarioId")]
        public virtual Funcionario Funcionario { get; set; } = null!;

        public virtual ICollection<ItemVenda> ItemVendas { get; set; } = new List<ItemVenda>();
    }
}
```

### 6. ItemVenda.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class ItemVenda
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Venda é obrigatória")]
        [Display(Name = "Venda")]
        public int VendaId { get; set; }

        [Required(ErrorMessage = "Produto é obrigatório")]
        [Display(Name = "Produto")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço Unitário")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Compra")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoCompra { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Lucro do Item")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LucroItem => (PrecoUnitario - PrecoCompra) * Quantidade;

        [NotMapped]
        [Display(Name = "Margem de Lucro")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public decimal MargemLucro => PrecoCompra > 0 ? ((PrecoUnitario - PrecoCompra) / PrecoCompra) : 0;

        // Relacionamentos
        [ForeignKey("VendaId")]
        public virtual Venda Venda { get; set; } = null!;

        [ForeignKey("ProdutoId")]
        public virtual Produto Produto { get; set; } = null!;
    }
}
```

### 7. Servico.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Servico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Display(Name = "Funcionário Responsável")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "Tipo de serviço é obrigatório")]
        [StringLength(100, ErrorMessage = "Tipo de serviço deve ter no máximo 100 caracteres")]
        [Display(Name = "Tipo de Serviço")]
        public string TipoServico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        [Display(Name = "Descrição do Serviço")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data do serviço é obrigatória")]
        [Display(Name = "Data do Serviço")]
        [DataType(DataType.DateTime)]
        public DateTime DataServico { get; set; } = DateTime.Now;

        [Display(Name = "Data de Conclusão")]
        [DataType(DataType.DateTime)]
        public DateTime? DataConclusao { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do serviço deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor do Serviço")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ValorServico { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(30, ErrorMessage = "Status deve ter no máximo 30 caracteres")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Agendado";

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Duração do Serviço")]
        public string DuracaoServico
        {
            get
            {
                if (DataConclusao.HasValue)
                {
                    var duracao = DataConclusao.Value - DataServico;
                    return $"{duracao.Days} dias e {duracao.Hours} horas";
                }
                return "Em andamento";
            }
        }

        [NotMapped]
        [Display(Name = "Status Class")]
        public string StatusClass => Status switch
        {
            "Agendado" => "warning",
            "Em Andamento" => "info",
            "Concluído" => "success",
            "Cancelado" => "danger",
            _ => "secondary"
        };

        // Relacionamentos
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; } = null!;

        [ForeignKey("FuncionarioId")]
        public virtual Funcionario Funcionario { get; set; } = null!;
    }
}
```

### 8. Enums.cs
```csharp
namespace SistemaEstoque.Models.Enums
{
    public enum StatusVenda
    {
        Pendente,
        Finalizada,
        Cancelada
    }

    public enum StatusServico
    {
        Agendado,
        EmAndamento,
        Concluido,
        Cancelado
    }

    public enum FormaPagamento
    {
        Dinheiro,
        CartaoCredito,
        CartaoDebito,
        PIX,
        Transferencia,
        Boleto
    }

    public enum TipoMovimentacao
    {
        Entrada,
        Saida,
        Ajuste
    }
}
```

## 📊 ViewModels (pasta ViewModels)

### 1. DashboardViewModel.cs
```csharp
namespace SistemaEstoque.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProdutos { get; set; }
        public int ProdutosEstoqueBaixo { get; set; }
        public int TotalFuncionarios { get; set; }
        public int TotalClientes { get; set; }
        
        public decimal VendasHoje { get; set; }
        public decimal VendasMes { get; set; }
        public decimal LucroMes { get; set; }
        
        public List<Produto> ProdutosMaisVendidos { get; set; } = new();
        public List<Funcionario> FuncionariosTop { get; set; } = new();
        public List<Venda> UltimasVendas { get; set; } = new();
        public List<Servico> ServicosAgendados { get; set; } = new();
        
        // Dados para gráficos
        public List<VendasPorDiaViewModel> VendasPorDia { get; set; } = new();
        public List<VendasPorCategoriaViewModel> VendasPorCategoria { get; set; } = new();
    }

    public class VendasPorDiaViewModel
    {
        public string Data { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    public class VendasPorCategoriaViewModel
    {
        public string Categoria { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
    }
}
```

### 2. VendaViewModel.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Models.ViewModels
{
    public class VendaViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }
        
        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }
        
        [Required(ErrorMessage = "Data da venda é obrigatória")]
        [Display(Name = "Data da Venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = string.Empty;
        
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        // Lista de itens da venda
        public List<ItemVendaViewModel> Itens { get; set; } = new();
        
        // Lista para dropdowns
        public List<Cliente> Clientes { get; set; } = new();
        public List<Funcionario> Funcionarios { get; set; } = new();
        public List<Produto> Produtos { get; set; } = new();
        
        // Propriedades calculadas
        public decimal ValorTotal => Itens.Sum(i => i.Subtotal);
        public int QuantidadeItens => Itens.Sum(i => i.Quantidade);
    }

    public class ItemVendaViewModel
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}
```

## 📋 Lista de Verificação de Models

- [x] **Categoria** - Categorização de produtos
- [x] **Produto** - Produtos/peças do estoque
- [x] **Cliente** - Cadastro de clientes
- [x] **Funcionario** - Gestão de funcionários
- [x] **Venda** - Registro de vendas
- [x] **ItemVenda** - Itens de cada venda
- [x] **Servico** - Serviços prestados
- [x] **Enums** - Enumerações do sistema
- [x] **ViewModels** - Models para views específicas

## 🔧 Configurações Adicionais

### ApplicationDbContext.cs (atualizado)
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configurações de precisão decimal
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

    // Índices únicos
    modelBuilder.Entity<Funcionario>()
        .HasIndex(f => f.CPF)
        .IsUnique();

    modelBuilder.Entity<Funcionario>()
        .HasIndex(f => f.Email)
        .IsUnique();

    // Relacionamentos
    modelBuilder.Entity<ItemVenda>()
        .HasOne(iv => iv.Venda)
        .WithMany(v => v.ItemVendas)
        .HasForeignKey(iv => iv.VendaId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<ItemVenda>()
        .HasOne(iv => iv.Produto)
        .WithMany(p => p.ItemVendas)
        .HasForeignKey(iv => iv.ProdutoId)
        .OnDelete(DeleteBehavior.Restrict);

    // Seed data inicial
    SeedData(modelBuilder);
}

private void SeedData(ModelBuilder modelBuilder)
{
    // Categorias iniciais
    modelBuilder.Entity<Categoria>().HasData(
        new Categoria { Id = 1, Nome = "Peças de Geladeira", Descricao = "Componentes para refrigeradores", Ativo = true },
        new Categoria { Id = 2, Nome = "Peças de Fogão", Descricao = "Componentes para fogões", Ativo = true },
        new Categoria { Id = 3, Nome = "Peças de Microondas", Descricao = "Componentes para microondas", Ativo = true },
        new Categoria { Id = 4, Nome = "Ferramentas", Descricao = "Ferramentas para manutenção", Ativo = true }
    );
}
```

Estes models fornecem uma base sólida para o sistema de estoque, com validações apropriadas, relacionamentos bem definidos e propriedades calculadas úteis para a interface do usuário.
