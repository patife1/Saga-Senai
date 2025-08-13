using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;
using SistemaEstoque.ViewModels;

namespace SistemaEstoque.Controllers
{
    public class VendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vendas
        public async Task<IActionResult> Index(DateTime? dataInicio, DateTime? dataFim, int? clienteId, int? funcionarioId)
        {
            var query = _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.ItemVendas)
                .ThenInclude(i => i.Produto)
                .AsQueryable();

            // Aplicar filtros
            if (dataInicio.HasValue)
                query = query.Where(v => v.DataVenda.Date >= dataInicio.Value.Date);
            
            if (dataFim.HasValue)
                query = query.Where(v => v.DataVenda.Date <= dataFim.Value.Date);
            
            if (clienteId.HasValue)
                query = query.Where(v => v.ClienteId == clienteId.Value);
            
            if (funcionarioId.HasValue)
                query = query.Where(v => v.FuncionarioId == funcionarioId.Value);

            var vendas = await query.OrderByDescending(v => v.DataVenda).ToListAsync();

            // Converter para ViewModels
            var vendasViewModel = vendas.Select(v => new VendaViewModel
            {
                Id = v.Id,
                DataVenda = v.DataVenda,
                ClienteId = v.ClienteId,
                FuncionarioId = v.FuncionarioId,
                FormaPagamento = v.FormaPagamento,
                Observacoes = v.Observacoes,
                ValorTotal = v.ValorTotal,
                Cliente = v.Cliente,
                Funcionario = v.Funcionario,
                Itens = v.ItemVendas.Select(i => new ItemVendaViewModel
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal,
                    NomeProduto = i.Produto?.Nome,
                    CodigoProduto = i.Produto?.CodigoBarras,
                    Produto = i.Produto
                }).ToList()
            }).ToList();

            // Dados para os filtros
            ViewBag.Clientes = new SelectList(
                await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(),
                "Id", "Nome", clienteId);
            
            ViewBag.Funcionarios = new SelectList(
                await _context.Funcionarios.OrderBy(f => f.Nome).ToListAsync(),
                "Id", "Nome", funcionarioId);

            // Estatísticas
            ViewBag.TotalVendas = vendas.Count;
            ViewBag.FaturamentoTotal = vendas.Sum(v => v.ValorTotal);
            ViewBag.TicketMedio = vendas.Any() ? vendas.Average(v => v.ValorTotal) : 0;
            ViewBag.ItensVendidos = vendas.SelectMany(v => v.ItemVendas).Sum(i => i.Quantidade);

            // Manter filtros na view
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(vendasViewModel);
        }

        // GET: Vendas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.ItemVendas)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venda == null)
            {
                return NotFound();
            }

            // Converter para ViewModel
            var vendaViewModel = new VendaViewModel
            {
                Id = venda.Id,
                DataVenda = venda.DataVenda,
                ClienteId = venda.ClienteId,
                FuncionarioId = venda.FuncionarioId,
                FormaPagamento = venda.FormaPagamento,
                Observacoes = venda.Observacoes,
                ValorTotal = venda.ValorTotal,
                Cliente = venda.Cliente,
                Funcionario = venda.Funcionario,
                Itens = venda.ItemVendas.Select(i => new ItemVendaViewModel
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal,
                    NomeProduto = i.Produto?.Nome,
                    CodigoProduto = i.Produto?.CodigoBarras,
                    Produto = i.Produto
                }).ToList()
            };

            return View(vendaViewModel);
        }

        // GET: Vendas/Imprimir/5
        public async Task<IActionResult> Imprimir(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.ItemVendas)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venda == null)
            {
                return NotFound();
            }

            // Converter para ViewModel
            var vendaViewModel = new VendaViewModel
            {
                Id = venda.Id,
                DataVenda = venda.DataVenda,
                ClienteId = venda.ClienteId,
                FuncionarioId = venda.FuncionarioId,
                FormaPagamento = venda.FormaPagamento,
                Observacoes = venda.Observacoes,
                ValorTotal = venda.ValorTotal,
                Cliente = venda.Cliente,
                Funcionario = venda.Funcionario,
                Itens = venda.ItemVendas.Select(i => new ItemVendaViewModel
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal,
                    NomeProduto = i.Produto?.Nome,
                    CodigoProduto = i.Produto?.CodigoBarras,
                    Produto = i.Produto
                }).ToList()
            };

            return View(vendaViewModel);
        }

        // GET: Vendas/Create - PDV (Point of Sale)
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(c => c.Ativo), "Id", "Nome");
            ViewData["FuncionarioId"] = new SelectList(_context.Funcionarios.Where(f => f.Ativo), "Id", "Nome");
            
            var vendaViewModel = new VendaViewModel
            {
                DataVenda = DateTime.Now,
                FormaPagamento = "Dinheiro"
            };

            return View(vendaViewModel);
        }

        // POST: Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaViewModel vendaViewModel)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Criar a venda
                    var venda = new Venda
                    {
                        ClienteId = vendaViewModel.ClienteId,
                        FuncionarioId = vendaViewModel.FuncionarioId,
                        DataVenda = vendaViewModel.DataVenda,
                        FormaPagamento = vendaViewModel.FormaPagamento,
                        Status = vendaViewModel.Status,
                        Observacoes = vendaViewModel.Observacoes,
                        ValorTotal = 0 // Será calculado após adicionar itens
                    };

                    _context.Vendas.Add(venda);
                    await _context.SaveChangesAsync();

                    // Adicionar itens da venda
                    decimal valorTotal = 0;
                    foreach (var item in vendaViewModel.Itens)
                    {
                        if (item.Quantidade > 0 && item.ProdutoId > 0)
                        {
                            var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                            if (produto == null)
                            {
                                ModelState.AddModelError("", $"Produto com ID {item.ProdutoId} não encontrado.");
                                continue;
                            }

                            // Verificar estoque
                            if (produto.QuantidadeEstoque < item.Quantidade)
                            {
                                ModelState.AddModelError("", $"Estoque insuficiente para {produto.Nome}. Disponível: {produto.QuantidadeEstoque}");
                                await transaction.RollbackAsync();
                                ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(c => c.Ativo), "Id", "Nome", vendaViewModel.ClienteId);
                                ViewData["FuncionarioId"] = new SelectList(_context.Funcionarios.Where(f => f.Ativo), "Id", "Nome", vendaViewModel.FuncionarioId);
                                return View(vendaViewModel);
                            }

                            var itemVenda = new ItemVenda
                            {
                                VendaId = venda.Id,
                                ProdutoId = item.ProdutoId,
                                Quantidade = item.Quantidade,
                                PrecoUnitario = item.PrecoUnitario > 0 ? item.PrecoUnitario : produto.PrecoVenda,
                                PrecoCompra = produto.PrecoCompra
                            };

                            itemVenda.Subtotal = itemVenda.Quantidade * itemVenda.PrecoUnitario;
                            valorTotal += itemVenda.Subtotal;

                            _context.ItemVendas.Add(itemVenda);

                            // Atualizar estoque
                            produto.QuantidadeEstoque -= item.Quantidade;
                            _context.Produtos.Update(produto);
                        }
                    }

                    // Atualizar valor total da venda
                    venda.ValorTotal = valorTotal;
                    _context.Vendas.Update(venda);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Venda realizada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = venda.Id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Erro ao processar venda: " + ex.Message);
                }
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(c => c.Ativo), "Id", "Nome", vendaViewModel.ClienteId);
            ViewData["FuncionarioId"] = new SelectList(_context.Funcionarios.Where(f => f.Ativo), "Id", "Nome", vendaViewModel.FuncionarioId);
            return View(vendaViewModel);
        }

        // API para buscar produtos no PDV
        [HttpGet]
        public async Task<IActionResult> BuscarProdutos(string termo)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && 
                           (p.Nome.Contains(termo) || 
                            (p.CodigoBarras != null && p.CodigoBarras.Contains(termo)) ||
                            p.Categoria.Nome.Contains(termo)))
                .Select(p => new
                {
                    id = p.Id,
                    nome = p.Nome,
                    categoria = p.Categoria.Nome,
                    preco = p.PrecoVenda,
                    estoque = p.QuantidadeEstoque,
                    codigoBarras = p.CodigoBarras
                })
                .Take(10)
                .ToListAsync();

            return Json(produtos);
        }

        // API para obter dados do produto por código de barras
        [HttpGet]
        public async Task<IActionResult> ProdutoPorCodigo(string codigo)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && p.CodigoBarras == codigo)
                .Select(p => new
                {
                    id = p.Id,
                    nome = p.Nome,
                    categoria = p.Categoria.Nome,
                    preco = p.PrecoVenda,
                    estoque = p.QuantidadeEstoque,
                    codigoBarras = p.CodigoBarras
                })
                .FirstOrDefaultAsync();

            if (produto == null)
            {
                return NotFound();
            }

            return Json(produto);
        }

        // GET: Vendas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.ItemVendas)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // POST: Vendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.ItemVendas)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda != null)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Reverter estoque
                    foreach (var item in venda.ItemVendas)
                    {
                        item.Produto.QuantidadeEstoque += item.Quantidade;
                        _context.Produtos.Update(item.Produto);
                    }

                    // Cancelar venda
                    venda.Status = "Cancelada";
                    _context.Vendas.Update(venda);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Venda cancelada e estoque revertido com sucesso!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Erro ao cancelar venda: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VendaExists(int id)
        {
            return _context.Vendas.Any(e => e.Id == id);
        }
    }
}
