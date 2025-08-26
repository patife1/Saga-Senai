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
                                             (p.Descricao != null && p.Descricao.Contains(searchString)));
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
        public async Task<IActionResult> Create()
        {
            Console.WriteLine("=== DEBUG PRODUTO CREATE GET ===");
            
            var categorias = await _context.Categorias.Where(c => c.Ativo).ToListAsync();
            Console.WriteLine($"Categorias ativas encontradas: {categorias.Count}");
            
            foreach (var cat in categorias)
            {
                Console.WriteLine($"Categoria ID: {cat.Id}, Nome: {cat.Nome}");
            }
            
            ViewData["CategoriaId"] = new SelectList(categorias, "Id", "Nome");
            Console.WriteLine("ViewData[\"CategoriaId\"] configurado com sucesso");
            
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,CategoriaId,QuantidadeEstoque,PrecoCompra,PrecoVenda,EstoqueMinimo,CodigoBarras,Ativo")] Produto produto)
        {
            Console.WriteLine("=== DEBUG PRODUTO CREATE ===");
            Console.WriteLine($"Produto recebido: Nome={produto.Nome}, CategoriaId={produto.CategoriaId}");
            Console.WriteLine($"PrecoCompra={produto.PrecoCompra}, PrecoVenda={produto.PrecoVenda}");
            Console.WriteLine($"QuantidadeEstoque={produto.QuantidadeEstoque}, EstoqueMinimo={produto.EstoqueMinimo}");
            
            // Remover validação da propriedade Categoria se existir
            if (ModelState.ContainsKey("Categoria"))
            {
                Console.WriteLine("Removendo validação da propriedade Categoria...");
                ModelState.Remove("Categoria");
            }
            
            // Verificar se a categoria existe
            var categoriaExists = await _context.Categorias.AnyAsync(c => c.Id == produto.CategoriaId && c.Ativo);
            if (!categoriaExists)
            {
                Console.WriteLine($"ERRO: Categoria com ID {produto.CategoriaId} não existe ou está inativa");
                ModelState.AddModelError("CategoriaId", "Categoria selecionada não é válida");
            }
            
            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState é VÁLIDO - Tentando salvar produto...");
                try
                {
                    produto.DataCadastro = DateTime.Now;
                    _context.Add(produto);
                    Console.WriteLine("Produto adicionado ao contexto...");
                    
                    var result = await _context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync retornou: {result} registros afetados");
                    
                    TempData["Success"] = "Produto criado com sucesso!";
                    Console.WriteLine("Produto salvo com sucesso! Redirecionando...");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERRO ao salvar produto: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    
                    ModelState.AddModelError("", $"Erro ao salvar produto: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("ModelState é INVÁLIDO:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Campo: {error.Key}");
                    foreach (var errorMsg in error.Value.Errors)
                    {
                        Console.WriteLine($"  - Erro: {errorMsg.ErrorMessage}");
                    }
                }
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
            Console.WriteLine("=== DEBUG PRODUTO EDIT ===");
            Console.WriteLine($"ID do produto: {id}");
            Console.WriteLine($"Produto recebido: Nome={produto.Nome}, CategoriaId={produto.CategoriaId}");
            Console.WriteLine($"PrecoCompra={produto.PrecoCompra}, PrecoVenda={produto.PrecoVenda}");
            
            if (id != produto.Id)
            {
                Console.WriteLine($"ERRO: ID não confere. URL ID={id}, Produto ID={produto.Id}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState é VÁLIDO - Tentando atualizar produto...");
                try
                {
                    _context.Update(produto);
                    Console.WriteLine("Produto marcado para atualização...");
                    
                    var result = await _context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync retornou: {result} registros afetados");
                    
                    TempData["Success"] = "Produto atualizado com sucesso!";
                    Console.WriteLine("Produto atualizado com sucesso! Redirecionando...");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"ERRO de concorrência: {ex.Message}");
                    if (!ProdutoExists(produto.Id))
                    {
                        Console.WriteLine("Produto não existe mais no banco");
                        return NotFound();
                    }
                    else
                    {
                        Console.WriteLine("Re-lançando exceção de concorrência");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERRO geral ao atualizar produto: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    
                    ModelState.AddModelError("", $"Erro ao atualizar produto: {ex.Message}");
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("ModelState é INVÁLIDO para Edit:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Campo: {error.Key}");
                    foreach (var errorMsg in error.Value.Errors)
                    {
                        Console.WriteLine($"  - Erro: {errorMsg.ErrorMessage}");
                    }
                }
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
