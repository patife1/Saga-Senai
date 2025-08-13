using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Produtos
        public async Task<IActionResult> Index(string searchString, int? categoriaId)
        {
            var produtos = _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo);

            if (!string.IsNullOrEmpty(searchString))
            {
                produtos = produtos.Where(p => p.Nome.Contains(searchString) || 
                                             p.Descricao.Contains(searchString));
            }

            if (categoriaId.HasValue)
            {
                produtos = produtos.Where(p => p.CategoriaId == categoriaId);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.Where(c => c.Ativo), "Id", "Nome", categoriaId);

            return View(await produtos.OrderBy(p => p.Nome).ToListAsync());
        }

        // GET: Produtos/Details/5
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

        // GET: Produtos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.Where(c => c.Ativo), "Id", "Nome");
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,CategoriaId,QuantidadeEstoque,PrecoCompra,PrecoVenda,EstoqueMinimo,CodigoBarras,Ativo")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                produto.DataCadastro = DateTime.Now;
                _context.Add(produto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produto criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.Where(c => c.Ativo), "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Edit/5
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.Where(c => c.Ativo), "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // POST: Produtos/Edit/5
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.Where(c => c.Ativo), "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Delete/5
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

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                // Exclusão lógica
                produto.Ativo = false;
                _context.Update(produto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produto desativado com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Produtos/EstoqueBaixo
        public async Task<IActionResult> EstoqueBaixo()
        {
            var produtosBaixoEstoque = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo)
                .OrderBy(p => p.QuantidadeEstoque)
                .ToListAsync();

            return View(produtosBaixoEstoque);
        }

        // POST: Produtos/AtualizarEstoque
        [HttpPost]
        public async Task<IActionResult> AtualizarEstoque(int id, int novaQuantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            produto.QuantidadeEstoque = novaQuantidade;
            _context.Update(produto);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Estoque atualizado com sucesso!";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
