using SistemaEstoque.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.ViewModels
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

        [Required(ErrorMessage = "Status é obrigatório")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [Display(Name = "Valor Total")]
        public decimal ValorTotal { get; set; }

        // Lista de itens da venda
        public List<ItemVendaViewModel> Itens { get; set; } = new List<ItemVendaViewModel>();

        // Propriedades para exibição
        public Cliente? Cliente { get; set; }
        public Funcionario? Funcionario { get; set; }
        
        // Propriedades calculadas e auxiliares
        public string? ClienteNome => Cliente?.Nome;
        public string FuncionarioNome => Funcionario?.Nome ?? string.Empty;
        public decimal Total => ValorTotal > 0 ? ValorTotal : Itens.Sum(i => i.Subtotal);
        public int QuantidadeItens => Itens.Sum(i => i.Quantidade);
    }

    public class ItemVendaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Produto é obrigatório")]
        [Display(Name = "Produto")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        [Display(Name = "Preço Unitário")]
        public decimal PrecoUnitario { get; set; }

        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        // Para exibição
        public string? NomeProduto { get; set; }
        public string? ProdutoNome => NomeProduto; // Alias para compatibilidade
        public string? CodigoProduto { get; set; }
        public string? CodigoBarras => CodigoProduto; // Alias para compatibilidade
        public int EstoqueDisponivel { get; set; }
        
        // Propriedade para relacionamento
        public Produto? Produto { get; set; }
    }

    // ViewModel para PDV (Point of Sale)
    public class PDVViewModel
    {
        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = "Dinheiro";

        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        // Carrinho de compras
        public List<ItemCarrinhoViewModel> Carrinho { get; set; } = new List<ItemCarrinhoViewModel>();

        [Display(Name = "Total da Venda")]
        public decimal TotalVenda => Carrinho.Sum(i => i.Subtotal);
    }

    public class ItemCarrinhoViewModel
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal => PrecoUnitario * Quantidade;
        public int EstoqueDisponivel { get; set; }
        public string Categoria { get; set; } = string.Empty;
    }

    // ViewModel para relatórios de vendas
    public class RelatorioVendasViewModel
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<Venda> Vendas { get; set; } = new List<Venda>();
        public decimal TotalVendas => Vendas.Sum(v => v.ValorTotal);
        public int QuantidadeVendas => Vendas.Count;
        public decimal TicketMedio => QuantidadeVendas > 0 ? TotalVendas / QuantidadeVendas : 0;
        
        // Top produtos vendidos no período
        public List<TopProdutoVendidoViewModel> TopProdutos { get; set; } = new List<TopProdutoVendidoViewModel>();
        
        // Vendas por forma de pagamento
        public Dictionary<string, decimal> VendasPorFormaPagamento { get; set; } = new Dictionary<string, decimal>();
        
        // Vendas por funcionário
        public Dictionary<string, decimal> VendasPorFuncionario { get; set; } = new Dictionary<string, decimal>();
    }

    public class TopProdutoVendidoViewModel
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal MargemLucro { get; set; }
    }
}
