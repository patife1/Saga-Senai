using SistemaEstoque.Models;

namespace SistemaEstoque.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProdutos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalFuncionarios { get; set; }
        public int TotalCategorias { get; set; }
        public decimal VendasMesAtual { get; set; }
        public decimal VendasUltimos30Dias { get; set; }
        public decimal ServicosUltimos30Dias { get; set; }
        public List<Produto> ProdutosEstoqueBaixo { get; set; } = new();
        public List<Servico> ServicosPendentes { get; set; } = new();
        public List<Venda> UltimasVendas { get; set; } = new();
        public List<TopProdutoViewModel> TopProdutos { get; set; } = new();
    }

    public class TopProdutoViewModel
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
