using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
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
            var clientes = _context.Clientes.Where(c => c.Ativo);

            if (!string.IsNullOrEmpty(searchString))
            {
                clientes = clientes.Where(c => c.Nome.Contains(searchString) || 
                                             (c.CPF != null && c.CPF.Contains(searchString)) ||
                                             (c.Email != null && c.Email.Contains(searchString)));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await clientes.OrderBy(c => c.Nome).ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Nome,CPF,Email,Telefone,Endereco,Ativo")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Verificar se CPF já existe
                if (!string.IsNullOrEmpty(cliente.CPF))
                {
                    var cpfExiste = await _context.Clientes
                        .AnyAsync(c => c.CPF == cliente.CPF && c.Ativo);
                    
                    if (cpfExiste)
                    {
                        ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
                        return View(cliente);
                    }
                }

                cliente.DataCadastro = DateTime.Now;
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
                    // Verificar se CPF já existe em outro registro
                    if (!string.IsNullOrEmpty(cliente.CPF))
                    {
                        var cpfExiste = await _context.Clientes
                            .AnyAsync(c => c.CPF == cliente.CPF && c.Id != cliente.Id && c.Ativo);
                        
                        if (cpfExiste)
                        {
                            ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
                            return View(cliente);
                        }
                    }

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
                .Include(c => c.Vendas)
                .Include(c => c.Servicos)
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
            var cliente = await _context.Clientes
                .Include(c => c.Vendas)
                .Include(c => c.Servicos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente != null)
            {
                // Verificar se há vendas ou serviços associados
                if (cliente.Vendas.Any() || cliente.Servicos.Any())
                {
                    TempData["Error"] = "Não é possível excluir cliente com vendas ou serviços associados!";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                // Exclusão lógica
                cliente.Ativo = false;
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente desativado com sucesso!";
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
                        .ThenInclude(i => i.Produto)
                .Include(c => c.Servicos)
                    .ThenInclude(s => s.Funcionario)
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
