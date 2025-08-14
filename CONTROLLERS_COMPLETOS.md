# 🎮 Controllers Completos - Sistema de Estoque

## 📁 Estrutura dos Controllers

### 1. HomeController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models.ViewModels;
using System.Diagnostics;

namespace SistemaEstoque.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoje = DateTime.Today;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var dashboard = new DashboardViewModel
            {
                TotalProdutos = await _context.Produtos.CountAsync(p => p.Ativo),
                ProdutosEstoqueBaixo = await _context.Produtos
                    .CountAsync(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo),
                TotalFuncionarios = await _context.Funcionarios.CountAsync(f => f.Ativo),
                TotalClientes = await _context.Clientes.CountAsync(c => c.Ativo),

                VendasHoje = await _context.Vendas
                    .Where(v => v.DataVenda.Date == hoje)
                    .SumAsync(v => v.ValorTotal),

                VendasMes = await _context.Vendas
                    .Where(v => v.DataVenda >= inicioMes)
                    .SumAsync(v => v.ValorTotal),

                UltimasVendas = await _context.Vendas
                    .Include(v => v.Cliente)
                    .Include(v => v.Funcionario)
                    .OrderByDescending(v => v.DataVenda)
                    .Take(5)
                    .ToListAsync(),

                ServicosAgendados = await _context.Servicos
                    .Include(s => s.Cliente)
                    .Include(s => s.Funcionario)
                    .Where(s => s.Status == "Agendado" || s.Status == "Em Andamento")
                    .OrderBy(s => s.DataServico)
                    .Take(5)
                    .ToListAsync()
            };

            // Calcular lucro do mês
            var itensVendasMes = await _context.ItemVendas
                .Include(iv => iv.Venda)
                .Where(iv => iv.Venda.DataVenda >= inicioMes)
                .ToListAsync();

            dashboard.LucroMes = itensVendasMes.Sum(iv => iv.LucroItem);

            // Vendas por dia (últimos 7 dias)
            for (int i = 6; i >= 0; i--)
            {
                var data = hoje.AddDays(-i);
                var vendas = await _context.Vendas
                    .Where(v => v.DataVenda.Date == data)
                    .SumAsync(v => v.ValorTotal);

                dashboard.VendasPorDia.Add(new VendasPorDiaViewModel
                {
                    Data = data.ToString("dd/MM"),
                    Valor = vendas
                });
            }

            return View(dashboard);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
```

### 2. EstoqueController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [Authorize]
    public class EstoqueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstoqueController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Estoque
        public async Task<IActionResult> Index(string searchString, int? categoriaId, bool? estoqueBaixo)
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome");

            var produtos = _context.Produtos.Include(p => p.Categoria).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                produtos = produtos.Where(p => p.Nome.Contains(searchString) ||
                                             p.Descricao.Contains(searchString) ||
                                             p.CodigoBarras.Contains(searchString));
            }

            if (categoriaId.HasValue)
            {
                produtos = produtos.Where(p => p.CategoriaId == categoriaId);
            }

            if (estoqueBaixo == true)
            {
                produtos = produtos.Where(p => p.QuantidadeEstoque <= p.EstoqueMinimo);
            }

            produtos = produtos.Where(p => p.Ativo).OrderBy(p => p.Nome);

            ViewBag.SearchString = searchString;
            ViewBag.CategoriaId = categoriaId;
            ViewBag.EstoqueBaixo = estoqueBaixo;

            return View(await produtos.ToListAsync());
        }

        // GET: Estoque/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Estoque/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome");
            return View();
        }

        // POST: Estoque/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,CategoriaId,QuantidadeEstoque,PrecoCompra,PrecoVenda,EstoqueMinimo,CodigoBarras")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                produto.DataCadastro = DateTime.Now;
                produto.Ativo = true;
                _context.Add(produto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produto criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = new SelectList(await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Estoque/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            ViewBag.Categorias = new SelectList(await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // POST: Estoque/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,CategoriaId,QuantidadeEstoque,PrecoCompra,PrecoVenda,EstoqueMinimo,CodigoBarras,Ativo,DataCadastro")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Produto atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = new SelectList(await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Estoque/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Estoque/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                produto.Ativo = false; // Soft delete
                _context.Update(produto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produto removido com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Estoque/AjustarEstoque
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjustarEstoque(int id, int quantidade, string motivo)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return Json(new { success = false, message = "Produto não encontrado." });
            }

            produto.QuantidadeEstoque = quantidade;
            _context.Update(produto);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Estoque ajustado com sucesso!" });
        }

        // GET: Estoque/EstoqueBaixo
        public async Task<IActionResult> EstoqueBaixo()
        {
            var produtosEstoqueBaixo = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo)
                .OrderBy(p => p.QuantidadeEstoque)
                .ToListAsync();

            return View(produtosEstoqueBaixo);
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
```

### 3. CategoriasController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [Authorize(Roles = "Admin,Gerente")]
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync());
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Descricao")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                categoria.DataCadastro = DateTime.Now;
                categoria.Ativo = true;
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoria criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Ativo,DataCadastro")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Categoria atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                // Verificar se há produtos associados
                var temProdutos = await _context.Produtos.AnyAsync(p => p.CategoriaId == id && p.Ativo);
                if (temProdutos)
                {
                    TempData["Error"] = "Não é possível excluir uma categoria que possui produtos associados.";
                    return RedirectToAction(nameof(Index));
                }

                categoria.Ativo = false; // Soft delete
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoria removida com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}
```

### 4. FuncionariosController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [Authorize(Roles = "Admin,Gerente")]
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FuncionariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index(string searchString)
        {
            var funcionarios = _context.Funcionarios.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                funcionarios = funcionarios.Where(f => f.Nome.Contains(searchString) ||
                                                       f.CPF.Contains(searchString) ||
                                                       f.Email.Contains(searchString) ||
                                                       f.Cargo.Contains(searchString));
            }

            funcionarios = funcionarios.Where(f => f.Ativo).OrderBy(f => f.Nome);

            ViewBag.SearchString = searchString;

            return View(await funcionarios.ToListAsync());
        }

        // GET: Funcionarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios
                .Include(f => f.Vendas)
                .Include(f => f.Servicos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // GET: Funcionarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Funcionarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,CPF,Email,Telefone,Cargo,Salario,DataAdmissao")] Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                funcionario.Ativo = true;
                _context.Add(funcionario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Funcionário criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(funcionario);
        }

        // GET: Funcionarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }
            return View(funcionario);
        }

        // POST: Funcionarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,CPF,Email,Telefone,Cargo,Salario,DataAdmissao,DataDemissao,Ativo,UserId")] Funcionario funcionario)
        {
            if (id != funcionario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcionario);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Funcionário atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionarioExists(funcionario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(funcionario);
        }

        // GET: Funcionarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios
                .FirstOrDefaultAsync(m => m.Id == id);

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // POST: Funcionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario != null)
            {
                funcionario.Ativo = false; // Soft delete
                funcionario.DataDemissao = DateTime.Now;
                _context.Update(funcionario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Funcionário removido com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Funcionarios/Performance/5
        public async Task<IActionResult> Performance(int? id, int? mes, int? ano)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }

            mes ??= DateTime.Now.Month;
            ano ??= DateTime.Now.Year;

            var dataInicio = new DateTime(ano.Value, mes.Value, 1);
            var dataFim = dataInicio.AddMonths(1).AddDays(-1);

            var vendas = await _context.Vendas
                .Include(v => v.ItemVendas)
                .Where(v => v.FuncionarioId == id && v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                .ToListAsync();

            var servicos = await _context.Servicos
                .Where(s => s.FuncionarioId == id && s.DataServico >= dataInicio && s.DataServico <= dataFim)
                .ToListAsync();

            ViewBag.Funcionario = funcionario;
            ViewBag.Mes = mes;
            ViewBag.Ano = ano;
            ViewBag.TotalVendas = vendas.Sum(v => v.ValorTotal);
            ViewBag.TotalServicos = servicos.Sum(s => s.ValorServico);
            ViewBag.QuantidadeVendas = vendas.Count;
            ViewBag.QuantidadeServicos = servicos.Count;

            return View(vendas);
        }

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionarios.Any(e => e.Id == id);
        }
    }
}
```

### 5. ClientesController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(string searchString)
        {
            var clientes = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                clientes = clientes.Where(c => c.Nome.Contains(searchString) ||
                                              c.CPF.Contains(searchString) ||
                                              c.Email.Contains(searchString) ||
                                              c.Telefone.Contains(searchString));
            }

            clientes = clientes.Where(c => c.Ativo).OrderBy(c => c.Nome);

            ViewBag.SearchString = searchString;

            return View(await clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Vendas)
                .Include(c => c.Servicos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,CPF,Email,Telefone,Endereco")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.DataCadastro = DateTime.Now;
                cliente.Ativo = true;
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,CPF,Email,Telefone,Endereco,DataCadastro,Ativo")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cliente atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Ativo = false; // Soft delete
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente removido com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Historico/5
        public async Task<IActionResult> Historico(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Vendas)
                    .ThenInclude(v => v.ItemVendas)
                    .ThenInclude(iv => iv.Produto)
                .Include(c => c.Servicos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
```

### 6. VendasController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;
using SistemaEstoque.Models.ViewModels;
using System.Security.Claims;

namespace SistemaEstoque.Controllers
{
    [Authorize]
    public class VendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vendas
        public async Task<IActionResult> Index(DateTime? dataInicio, DateTime? dataFim, int? funcionarioId)
        {
            var vendas = _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.ItemVendas)
                .AsQueryable();

            if (dataInicio.HasValue)
            {
                vendas = vendas.Where(v => v.DataVenda.Date >= dataInicio.Value.Date);
            }

            if (dataFim.HasValue)
            {
                vendas = vendas.Where(v => v.DataVenda.Date <= dataFim.Value.Date);
            }

            if (funcionarioId.HasValue)
            {
                vendas = vendas.Where(v => v.FuncionarioId == funcionarioId);
            }

            vendas = vendas.OrderByDescending(v => v.DataVenda);

            ViewBag.Funcionarios = new SelectList(await _context.Funcionarios
                .Where(f => f.Ativo)
                .OrderBy(f => f.Nome)
                .ToListAsync(), "Id", "Nome");

            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
            ViewBag.FuncionarioId = funcionarioId;

            return View(await vendas.ToListAsync());
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
                    .ThenInclude(iv => iv.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // GET: Vendas/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new VendaViewModel
            {
                DataVenda = DateTime.Now,
                Clientes = await _context.Clientes.Where(c => c.Ativo).OrderBy(c => c.Nome).ToListAsync(),
                Funcionarios = await _context.Funcionarios.Where(f => f.Ativo).OrderBy(f => f.Nome).ToListAsync(),
                Produtos = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Where(p => p.Ativo && p.QuantidadeEstoque > 0)
                    .OrderBy(p => p.Nome)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.Itens.Any())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var venda = new Venda
                    {
                        ClienteId = viewModel.ClienteId,
                        FuncionarioId = viewModel.FuncionarioId,
                        DataVenda = viewModel.DataVenda,
                        FormaPagamento = viewModel.FormaPagamento,
                        Observacoes = viewModel.Observacoes,
                        Status = "Finalizada",
                        ValorTotal = viewModel.ValorTotal
                    };

                    _context.Vendas.Add(venda);
                    await _context.SaveChangesAsync();

                    foreach (var item in viewModel.Itens)
                    {
                        var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                        if (produto == null || produto.QuantidadeEstoque < item.Quantidade)
                        {
                            throw new InvalidOperationException($"Produto {item.NomeProduto} não tem estoque suficiente.");
                        }

                        var itemVenda = new ItemVenda
                        {
                            VendaId = venda.Id,
                            ProdutoId = item.ProdutoId,
                            Quantidade = item.Quantidade,
                            PrecoUnitario = item.PrecoUnitario,
                            Subtotal = item.Subtotal,
                            PrecoCompra = produto.PrecoCompra
                        };

                        _context.ItemVendas.Add(itemVenda);

                        // Baixar do estoque
                        produto.QuantidadeEstoque -= item.Quantidade;
                        _context.Update(produto);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Venda realizada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = venda.Id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Erro ao processar venda: {ex.Message}");
                }
            }

            // Recarregar dados para a view
            viewModel.Clientes = await _context.Clientes.Where(c => c.Ativo).OrderBy(c => c.Nome).ToListAsync();
            viewModel.Funcionarios = await _context.Funcionarios.Where(f => f.Ativo).OrderBy(f => f.Nome).ToListAsync();
            viewModel.Produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && p.QuantidadeEstoque > 0)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Vendas/PDV
        public async Task<IActionResult> PDV()
        {
            var viewModel = new VendaViewModel
            {
                DataVenda = DateTime.Now,
                Clientes = await _context.Clientes.Where(c => c.Ativo).OrderBy(c => c.Nome).ToListAsync(),
                Funcionarios = await _context.Funcionarios.Where(f => f.Ativo).OrderBy(f => f.Nome).ToListAsync(),
                Produtos = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Where(p => p.Ativo && p.QuantidadeEstoque > 0)
                    .OrderBy(p => p.Nome)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Vendas/BuscarProduto
        [HttpPost]
        public async Task<IActionResult> BuscarProduto(string termo)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo &&
                           p.QuantidadeEstoque > 0 &&
                           (p.Nome.Contains(termo) ||
                            p.CodigoBarras.Contains(termo)))
                .Select(p => new
                {
                    id = p.Id,
                    nome = p.Nome,
                    preco = p.PrecoVenda,
                    estoque = p.QuantidadeEstoque,
                    categoria = p.Categoria.Nome
                })
                .Take(10)
                .ToListAsync();

            return Json(produtos);
        }

        private bool VendaExists(int id)
        {
            return _context.Vendas.Any(e => e.Id == id);
        }
    }
}
```

### 7. ServicosController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [Authorize]
    public class ServicosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Servicos
        public async Task<IActionResult> Index(string status, DateTime? dataInicio, DateTime? dataFim)
        {
            var servicos = _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Funcionario)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                servicos = servicos.Where(s => s.Status == status);
            }

            if (dataInicio.HasValue)
            {
                servicos = servicos.Where(s => s.DataServico.Date >= dataInicio.Value.Date);
            }

            if (dataFim.HasValue)
            {
                servicos = servicos.Where(s => s.DataServico.Date <= dataFim.Value.Date);
            }

            servicos = servicos.OrderByDescending(s => s.DataServico);

            ViewBag.StatusOptions = new SelectList(new[]
            {
                new { Value = "", Text = "Todos" },
                new { Value = "Agendado", Text = "Agendado" },
                new { Value = "Em Andamento", Text = "Em Andamento" },
                new { Value = "Concluído", Text = "Concluído" },
                new { Value = "Cancelado", Text = "Cancelado" }
            }, "Value", "Text", status);

            ViewBag.Status = status;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(await servicos.ToListAsync());
        }

        // GET: Servicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Funcionario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (servico == null)
            {
                return NotFound();
            }

            return View(servico);
        }

        // GET: Servicos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = new SelectList(await _context.Clientes
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome");

            ViewBag.Funcionarios = new SelectList(await _context.Funcionarios
                .Where(f => f.Ativo)
                .OrderBy(f => f.Nome)
                .ToListAsync(), "Id", "Nome");

            ViewBag.TiposServico = new SelectList(new[]
            {
                "Manutenção Geladeira",
                "Manutenção Fogão",
                "Manutenção Microondas",
                "Manutenção Lavadora",
                "Instalação",
                "Diagnóstico",
                "Outros"
            });

            ViewBag.StatusOptions = new SelectList(new[]
            {
                "Agendado",
                "Em Andamento",
                "Concluído",
                "Cancelado"
            });

            return View();
        }

        // POST: Servicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,FuncionarioId,TipoServico,Descricao,DataServico,ValorServico,Status,Observacoes")] Servico servico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Serviço criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            // Recarregar dados para a view
            ViewBag.Clientes = new SelectList(await _context.Clientes
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome", servico.ClienteId);

            ViewBag.Funcionarios = new SelectList(await _context.Funcionarios
                .Where(f => f.Ativo)
                .OrderBy(f => f.Nome)
                .ToListAsync(), "Id", "Nome", servico.FuncionarioId);

            return View(servico);
        }

        // GET: Servicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
            {
                return NotFound();
            }

            ViewBag.Clientes = new SelectList(await _context.Clientes
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome", servico.ClienteId);

            ViewBag.Funcionarios = new SelectList(await _context.Funcionarios
                .Where(f => f.Ativo)
                .OrderBy(f => f.Nome)
                .ToListAsync(), "Id", "Nome", servico.FuncionarioId);

            return View(servico);
        }

        // POST: Servicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,FuncionarioId,TipoServico,Descricao,DataServico,DataConclusao,ValorServico,Status,Observacoes")] Servico servico)
        {
            if (id != servico.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Se o status mudou para "Concluído" e não tem data de conclusão, definir agora
                    if (servico.Status == "Concluído" && !servico.DataConclusao.HasValue)
                    {
                        servico.DataConclusao = DateTime.Now;
                    }

                    _context.Update(servico);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Serviço atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoExists(servico.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clientes = new SelectList(await _context.Clientes
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync(), "Id", "Nome", servico.ClienteId);

            ViewBag.Funcionarios = new SelectList(await _context.Funcionarios
                .Where(f => f.Ativo)
                .OrderBy(f => f.Nome)
                .ToListAsync(), "Id", "Nome", servico.FuncionarioId);

            return View(servico);
        }

        // POST: Servicos/AtualizarStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatus(int id, string status)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
            {
                return Json(new { success = false, message = "Serviço não encontrado." });
            }

            servico.Status = status;
            if (status == "Concluído" && !servico.DataConclusao.HasValue)
            {
                servico.DataConclusao = DateTime.Now;
            }

            _context.Update(servico);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Status atualizado com sucesso!" });
        }

        // GET: Servicos/Agenda
        public async Task<IActionResult> Agenda(DateTime? data)
        {
            data ??= DateTime.Today;

            var servicos = await _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Funcionario)
                .Where(s => s.DataServico.Date == data.Value.Date)
                .OrderBy(s => s.DataServico)
                .ToListAsync();

            ViewBag.DataSelecionada = data.Value;

            return View(servicos);
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.Id == id);
        }
    }
}
```

## 📋 Checklist de Controllers

- [x] **HomeController** - Dashboard principal
- [x] **EstoqueController** - Gestão de produtos
- [x] **CategoriasController** - Gestão de categorias
- [x] **FuncionariosController** - Gestão de funcionários
- [x] **ClientesController** - Gestão de clientes
- [x] **VendasController** - Sistema de vendas/PDV
- [x] **ServicosController** - Gestão de serviços

## 🔐 Recursos de Segurança Implementados

- ✅ **Autorização por Roles** - Admin, Gerente, Funcionario
- ✅ **CSRF Protection** - ValidateAntiForgeryToken
- ✅ **Soft Delete** - Exclusão lógica de registros
- ✅ **Validação de Dados** - ModelState validation
- ✅ **Transações** - Para operações críticas como vendas

## 🎯 Próximos Passos

Após implementar os controllers:

1. ✅ **Criar Views** correspondentes
2. ✅ **Implementar API Controllers** (opcional)
3. ✅ **Adicionar mais validações**
4. ✅ **Implementar relatórios**
5. ✅ **Testes unitários**
6. ✅ **Logging e monitoramento**

Estes controllers fornecem uma base completa para o sistema de estoque, com todas as funcionalidades essenciais implementadas seguindo as melhores práticas do ASP.NET Core MVC.
