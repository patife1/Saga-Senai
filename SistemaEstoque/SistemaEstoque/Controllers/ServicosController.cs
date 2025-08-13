using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    public class ServicosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Servicos
        public async Task<IActionResult> Index(string busca, string status)
        {
            var query = _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Funcionario)
                .AsQueryable();

            if (!string.IsNullOrEmpty(busca))
            {
                query = query.Where(s => s.TipoServico.Contains(busca) || 
                                        s.Descricao.Contains(busca) ||
                                        s.Cliente.Nome.Contains(busca));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            var servicos = await query.OrderByDescending(s => s.DataServico).ToListAsync();

            ViewBag.Busca = busca;
            ViewBag.Status = status;

            return View(servicos);
        }

        // GET: Servicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var servico = await _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Funcionario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (servico == null)
                return NotFound();

            return View(servico);
        }

        // GET: Servicos/Create
        public IActionResult Create()
        {
            CarregarViewBags();
            var servico = new Servico
            {
                DataServico = DateTime.Now,
                Status = "Agendado"
            };
            return View(servico);
        }

        // POST: Servicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,FuncionarioId,TipoServico,Descricao,DataServico,ValorServico,Status,Observacoes")] Servico servico)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(servico);
                    await _context.SaveChangesAsync();
                    TempData["Sucesso"] = $"Serviço '{servico.TipoServico}' agendado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o serviço: " + ex.Message);
                }
            }

            CarregarViewBags();
            return View(servico);
        }

        // GET: Servicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
                return NotFound();

            CarregarViewBags();
            return View(servico);
        }

        // POST: Servicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,FuncionarioId,TipoServico,Descricao,DataServico,DataConclusao,ValorServico,Status,Observacoes")] Servico servico)
        {
            if (id != servico.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servico);
                    await _context.SaveChangesAsync();
                    TempData["Sucesso"] = $"Serviço '{servico.TipoServico}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoExists(servico.Id))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar o serviço: " + ex.Message);
                }
            }

            CarregarViewBags();
            return View(servico);
        }

        // GET: Servicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var servico = await _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Funcionario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (servico == null)
                return NotFound();

            return View(servico);
        }

        // POST: Servicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var servico = await _context.Servicos.FindAsync(id);
                if (servico != null)
                {
                    _context.Servicos.Remove(servico);
                    await _context.SaveChangesAsync();
                    TempData["Sucesso"] = $"Serviço '{servico.TipoServico}' excluído com sucesso!";
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao excluir o serviço: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Servicos/ConcluirServico/5
        [HttpPost]
        public async Task<IActionResult> ConcluirServico(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
                return NotFound();

            servico.Status = "Concluído";
            servico.DataConclusao = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Serviço '{servico.TipoServico}' concluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Servicos/CancelarServico/5
        [HttpPost]
        public async Task<IActionResult> CancelarServico(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
                return NotFound();

            servico.Status = "Cancelado";

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Serviço '{servico.TipoServico}' cancelado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.Id == id);
        }

        private void CarregarViewBags()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["FuncionarioId"] = new SelectList(_context.Funcionarios, "Id", "Nome");
            
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Agendado", Text = "Agendado" },
                new SelectListItem { Value = "Em Andamento", Text = "Em Andamento" },
                new SelectListItem { Value = "Concluído", Text = "Concluído" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" }
            };
            ViewData["StatusList"] = statusList;
        }
    }
}
