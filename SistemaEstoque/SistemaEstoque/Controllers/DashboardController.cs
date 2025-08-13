using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;
using SistemaEstoque.ViewModels;

namespace SistemaEstoque.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                // Contadores gerais
                TotalProdutos = await _context.Produtos.CountAsync(p => p.Ativo),
                TotalClientes = await _context.Clientes.CountAsync(c => c.Ativo),
                TotalFuncionarios = await _context.Funcionarios.CountAsync(f => f.Ativo),
                TotalCategorias = await _context.Categorias.CountAsync(c => c.Ativo),

                // Produtos com estoque baixo
                ProdutosEstoqueBaixo = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Where(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo)
                    .OrderBy(p => p.QuantidadeEstoque)
                    .Take(10)
                    .ToListAsync(),

                // Vendas do mês atual
                VendasMesAtual = await _context.Vendas
                    .Where(v => v.DataVenda.Month == DateTime.Now.Month && 
                               v.DataVenda.Year == DateTime.Now.Year)
                    .SumAsync(v => v.ValorTotal),

                // Serviços pendentes
                ServicosPendentes = await _context.Servicos
                    .Include(s => s.Cliente)
                    .Include(s => s.Funcionario)
                    .Where(s => s.Status == "Pendente" || s.Status == "Em Andamento")
                    .OrderBy(s => s.DataServico)
                    .Take(10)
                    .ToListAsync(),

                // Últimas vendas
                UltimasVendas = await _context.Vendas
                    .Include(v => v.Cliente)
                    .Include(v => v.Funcionario)
                    .OrderByDescending(v => v.DataVenda)
                    .Take(5)
                    .ToListAsync(),

                // Top produtos mais vendidos no mês
                TopProdutos = await _context.ItemVendas
                    .Include(i => i.Produto)
                    .Include(i => i.Venda)
                    .Where(i => i.Venda.DataVenda.Month == DateTime.Now.Month &&
                               i.Venda.DataVenda.Year == DateTime.Now.Year)
                    .GroupBy(i => new { i.ProdutoId, i.Produto.Nome })
                    .Select(g => new TopProdutoViewModel
                    {
                        ProdutoId = g.Key.ProdutoId,
                        NomeProduto = g.Key.Nome,
                        QuantidadeVendida = g.Sum(i => i.Quantidade),
                        ValorTotal = g.Sum(i => i.Subtotal)
                    })
                    .OrderByDescending(t => t.QuantidadeVendida)
                    .Take(5)
                    .ToListAsync()
            };

            // Calcular estatísticas por período
            var dataInicio = DateTime.Now.AddDays(-30);
            viewModel.VendasUltimos30Dias = await _context.Vendas
                .Where(v => v.DataVenda >= dataInicio)
                .SumAsync(v => v.ValorTotal);

            viewModel.ServicosUltimos30Dias = await _context.Servicos
                .Where(s => s.DataServico >= dataInicio)
                .SumAsync(s => s.ValorServico);

            return View(viewModel);
        }

        public IActionResult Relatorios()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RelatorioVendas(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.ItemVendas)
                    .ThenInclude(i => i.Produto)
                .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            ViewData["DataInicio"] = dataInicio.ToString("dd/MM/yyyy");
            ViewData["DataFim"] = dataFim.ToString("dd/MM/yyyy");
            ViewData["TotalVendas"] = vendas.Sum(v => v.ValorTotal);
            ViewData["QuantidadeVendas"] = vendas.Count;

            return View("RelatorioVendas", vendas);
        }

        [HttpPost]
        public async Task<IActionResult> RelatorioEstoque()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo)
                .OrderBy(p => p.Categoria.Nome)
                .ThenBy(p => p.Nome)
                .ToListAsync();

            var valorTotalEstoque = produtos.Sum(p => p.QuantidadeEstoque * p.PrecoCompra);
            var produtosEstoqueBaixo = produtos.Count(p => p.QuantidadeEstoque <= p.EstoqueMinimo);

            ViewData["ValorTotalEstoque"] = valorTotalEstoque;
            ViewData["ProdutosEstoqueBaixo"] = produtosEstoqueBaixo;

            return View("RelatorioEstoque", produtos);
        }
    }
}
