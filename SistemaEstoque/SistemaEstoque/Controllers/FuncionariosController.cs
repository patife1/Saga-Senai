using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [Authorize]
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FuncionariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index(string searchString)
        {
            var funcionarios = _context.Funcionarios.Where(f => f.Ativo);

            if (!string.IsNullOrEmpty(searchString))
            {
                funcionarios = funcionarios.Where(f => f.Nome.Contains(searchString) || 
                                                      f.CPF.Contains(searchString) ||
                                                      f.Email.Contains(searchString) ||
                                                      f.Cargo.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await funcionarios.OrderBy(f => f.Nome).ToListAsync());
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
                    .ThenInclude(s => s.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id && m.Ativo);

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
        public async Task<IActionResult> Create([Bind("Nome,CPF,Email,Telefone,Cargo,Salario,DataAdmissao,Ativo")] Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                // Verificar se CPF já existe
                var cpfExiste = await _context.Funcionarios
                    .AnyAsync(f => f.CPF == funcionario.CPF && f.Ativo);
                
                if (cpfExiste)
                {
                    ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
                    return View(funcionario);
                }

                // Verificar se Email já existe
                var emailExiste = await _context.Funcionarios
                    .AnyAsync(f => f.Email == funcionario.Email && f.Ativo);
                
                if (emailExiste)
                {
                    ModelState.AddModelError("Email", "Este email já está cadastrado.");
                    return View(funcionario);
                }

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
                    // Verificar se CPF já existe em outro registro
                    var cpfExiste = await _context.Funcionarios
                        .AnyAsync(f => f.CPF == funcionario.CPF && f.Id != funcionario.Id && f.Ativo);
                    
                    if (cpfExiste)
                    {
                        ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
                        return View(funcionario);
                    }

                    // Verificar se Email já existe em outro registro
                    var emailExiste = await _context.Funcionarios
                        .AnyAsync(f => f.Email == funcionario.Email && f.Id != funcionario.Id && f.Ativo);
                    
                    if (emailExiste)
                    {
                        ModelState.AddModelError("Email", "Este email já está cadastrado.");
                        return View(funcionario);
                    }

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
                .Include(f => f.Vendas)
                .Include(f => f.Servicos)
                .FirstOrDefaultAsync(m => m.Id == id && m.Ativo);

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
            var funcionario = await _context.Funcionarios
                .Include(f => f.Vendas)
                .Include(f => f.Servicos)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcionario != null)
            {
                // Verificar se há vendas ou serviços associados
                if (funcionario.Vendas.Any() || funcionario.Servicos.Any())
                {
                    TempData["Error"] = "Não é possível excluir funcionário com vendas ou serviços associados!";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                // Exclusão lógica - demissão
                funcionario.Ativo = false;
                funcionario.DataDemissao = DateTime.Now;
                _context.Update(funcionario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Funcionário desligado com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Funcionarios/Desempenho/5
        public async Task<IActionResult> Desempenho(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios
                .Include(f => f.Vendas)
                    .ThenInclude(v => v.ItemVendas)
                .Include(f => f.Servicos)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcionario == null)
            {
                return NotFound();
            }

            // Calcular estatísticas do funcionário
            var totalVendas = funcionario.Vendas.Sum(v => v.ValorTotal);
            var totalServicos = funcionario.Servicos.Sum(s => s.ValorServico);
            var servicosConcluidos = funcionario.Servicos.Count(s => s.Status == "Concluído");

            ViewData["TotalVendas"] = totalVendas;
            ViewData["TotalServicos"] = totalServicos;
            ViewData["ServicosConcluidos"] = servicosConcluidos;
            ViewData["TotalFaturamento"] = totalVendas + totalServicos;

            return View(funcionario);
        }

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionarios.Any(e => e.Id == id);
        }
    }
}
