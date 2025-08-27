using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace SistemaEstoque.Controllers
{
    [Authorize]
    public class RelatoriosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RelatoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Relatorios
        public IActionResult Index()
        {
            return View();
        }

        // GET: Relatorios/Vendas
        public async Task<IActionResult> RelatorioVendas(DateTime? dataInicio, DateTime? dataFim)
        {
            // Se não informado, usar últimos 30 dias
            dataInicio ??= DateTime.Now.AddDays(-30);
            dataFim ??= DateTime.Now;

            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.ItemVendas)
                    .ThenInclude(i => i.Produto)
                .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            // Gerar PDF
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Configurar fonte
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Cabeçalho
            var titulo = new Paragraph("RELATÓRIO DE VENDAS")
                .SetFont(boldFont)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);
            document.Add(titulo);

            var periodo = new Paragraph($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                .SetFont(font)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);
            document.Add(periodo);

            // Resumo
            var totalVendas = vendas.Sum(v => v.ValorTotal);
            var quantidadeVendas = vendas.Count;

            var resumo = new Paragraph($"Total de Vendas: {quantidadeVendas} | Valor Total: {totalVendas:C}")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(10)
                .SetMarginBottom(20);
            document.Add(resumo);

            // Tabela de vendas
            if (vendas.Any())
            {
                var table = new Table(6, true);
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Cabeçalho da tabela
                table.AddHeaderCell(new Cell().Add(new Paragraph("Data").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Cliente").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Funcionário").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Forma Pagamento").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Valor Total").SetFont(boldFont)));

                // Dados das vendas
                foreach (var venda in vendas)
                {
                    table.AddCell(new Cell().Add(new Paragraph(venda.DataVenda.ToString("dd/MM/yyyy")).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(venda.Cliente?.Nome ?? "N/A").SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(venda.Funcionario.Nome).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(venda.FormaPagamento).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(venda.Status).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(venda.ValorTotal.ToString("C")).SetFont(font)));
                }

                document.Add(table);
            }
            else
            {
                var semDados = new Paragraph("Nenhuma venda encontrada no período informado.")
                    .SetFont(font)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(50);
                document.Add(semDados);
            }

            // Rodapé
            var rodape = new Paragraph($"Relatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(font)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFixedPosition(40, 30, 500);
            document.Add(rodape);

            document.Close();

            return File(stream.ToArray(), "application/pdf", $"relatorio_vendas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        // GET: Relatorios/Produtos
        public async Task<IActionResult> RelatorioProdutos()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            // Gerar PDF
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Configurar fonte
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Cabeçalho
            var titulo = new Paragraph("RELATÓRIO DE PRODUTOS")
                .SetFont(boldFont)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);
            document.Add(titulo);

            // Resumo
            var totalProdutos = produtos.Count;
            var totalEstoque = produtos.Sum(p => p.QuantidadeEstoque);
            var valorTotalEstoque = produtos.Sum(p => p.ValorTotalEstoque);
            var produtosEstoqueBaixo = produtos.Count(p => p.EstoqueBaixo);

            var resumo = new Paragraph($"Total de Produtos: {totalProdutos} | Total em Estoque: {totalEstoque} | Valor Total: {valorTotalEstoque:C} | Estoque Baixo: {produtosEstoqueBaixo}")
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(10)
                .SetMarginBottom(20);
            document.Add(resumo);

            // Tabela de produtos
            if (produtos.Any())
            {
                var table = new Table(7, true);
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Cabeçalho da tabela
                table.AddHeaderCell(new Cell().Add(new Paragraph("Código").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Nome").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Categoria").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Estoque").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Preço Compra").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Preço Venda").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetFont(boldFont)));

                // Dados dos produtos
                foreach (var produto in produtos)
                {
                    var statusColor = produto.EstoqueBaixo ? ColorConstants.RED : ColorConstants.BLACK;
                    
                    table.AddCell(new Cell().Add(new Paragraph(produto.Id.ToString()).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(produto.Nome).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(produto.Categoria.Nome).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(produto.QuantidadeEstoque.ToString()).SetFont(font).SetFontColor(statusColor)));
                    table.AddCell(new Cell().Add(new Paragraph(produto.PrecoCompra.ToString("C")).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(produto.PrecoVenda.ToString("C")).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(produto.EstoqueBaixo ? "BAIXO" : "OK").SetFont(font).SetFontColor(statusColor)));
                }

                document.Add(table);
            }

            // Rodapé
            var rodape = new Paragraph($"Relatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(font)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFixedPosition(40, 30, 500);
            document.Add(rodape);

            document.Close();

            return File(stream.ToArray(), "application/pdf", $"relatorio_produtos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        // GET: Relatorios/Servicos
        public async Task<IActionResult> RelatorioServicos(DateTime? dataInicio, DateTime? dataFim)
        {
            // Se não informado, usar últimos 30 dias
            dataInicio ??= DateTime.Now.AddDays(-30);
            dataFim ??= DateTime.Now;

            var servicos = await _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Funcionario)
                .Where(s => s.DataServico >= dataInicio && s.DataServico <= dataFim)
                .OrderByDescending(s => s.DataServico)
                .ToListAsync();

            // Gerar PDF
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Configurar fonte
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Cabeçalho
            var titulo = new Paragraph("RELATÓRIO DE SERVIÇOS")
                .SetFont(boldFont)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);
            document.Add(titulo);

            var periodo = new Paragraph($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                .SetFont(font)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);
            document.Add(periodo);

            // Resumo
            var totalServicos = servicos.Count;
            var valorTotal = servicos.Sum(s => s.ValorServico);
            var servicosConcluidos = servicos.Count(s => s.Status == "Concluído");
            var servicosAndamento = servicos.Count(s => s.Status == "Em Andamento");

            var resumo = new Paragraph($"Total: {totalServicos} | Concluídos: {servicosConcluidos} | Em Andamento: {servicosAndamento} | Valor Total: {valorTotal:C}")
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(10)
                .SetMarginBottom(20);
            document.Add(resumo);

            // Tabela de serviços
            if (servicos.Any())
            {
                var table = new Table(6, true);
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Cabeçalho da tabela
                table.AddHeaderCell(new Cell().Add(new Paragraph("Data").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Tipo Serviço").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Cliente").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Funcionário").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Valor").SetFont(boldFont)));

                // Dados dos serviços
                foreach (var servico in servicos)
                {
                    table.AddCell(new Cell().Add(new Paragraph(servico.DataServico.ToString("dd/MM/yyyy")).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(servico.TipoServico).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(servico.Cliente.Nome).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(servico.Funcionario.Nome).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(servico.Status).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(servico.ValorServico.ToString("C")).SetFont(font)));
                }

                document.Add(table);
            }
            else
            {
                var semDados = new Paragraph("Nenhum serviço encontrado no período informado.")
                    .SetFont(font)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(50);
                document.Add(semDados);
            }

            // Rodapé
            var rodape = new Paragraph($"Relatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(font)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFixedPosition(40, 30, 500);
            document.Add(rodape);

            document.Close();

            return File(stream.ToArray(), "application/pdf", $"relatorio_servicos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        // GET: Relatorios/Dashboard
        public async Task<IActionResult> RelatorioDashboard()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var inicioAno = new DateTime(hoje.Year, 1, 1);

            // Dados para o relatório
            var totalProdutos = await _context.Produtos.CountAsync(p => p.Ativo);
            var totalClientes = await _context.Clientes.CountAsync(c => c.Ativo);
            var totalFuncionarios = await _context.Funcionarios.CountAsync(f => f.Ativo);
            
            var vendasMes = (await _context.Vendas
                .Where(v => v.DataVenda >= inicioMes)
                .ToListAsync())
                .Sum(v => v.ValorTotal);
            
            var vendasAno = (await _context.Vendas
                .Where(v => v.DataVenda >= inicioAno)
                .ToListAsync())
                .Sum(v => v.ValorTotal);

            var servicosMes = (await _context.Servicos
                .Where(s => s.DataServico >= inicioMes)
                .ToListAsync())
                .Sum(s => s.ValorServico);

            var produtosEstoqueBaixo = await _context.Produtos
                .Where(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo)
                .CountAsync();

            // Gerar PDF
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Configurar fonte
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Cabeçalho
            var titulo = new Paragraph("RELATÓRIO DASHBOARD")
                .SetFont(boldFont)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(30);
            document.Add(titulo);

            // Resumo Geral
            var resumoGeral = new Paragraph("RESUMO GERAL")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetMarginBottom(10);
            document.Add(resumoGeral);

            var dadosGerais = new Paragraph($"• Total de Produtos Ativos: {totalProdutos}\n" +
                                          $"• Total de Clientes Ativos: {totalClientes}\n" +
                                          $"• Total de Funcionários Ativos: {totalFuncionarios}\n" +
                                          $"• Produtos com Estoque Baixo: {produtosEstoqueBaixo}")
                .SetFont(font)
                .SetFontSize(12)
                .SetMarginBottom(20);
            document.Add(dadosGerais);

            // Vendas
            var tituloVendas = new Paragraph("VENDAS")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetMarginBottom(10);
            document.Add(tituloVendas);

            var dadosVendas = new Paragraph($"• Vendas do Mês ({inicioMes:MM/yyyy}): {vendasMes:C}\n" +
                                          $"• Vendas do Ano ({inicioAno.Year}): {vendasAno:C}")
                .SetFont(font)
                .SetFontSize(12)
                .SetMarginBottom(20);
            document.Add(dadosVendas);

            // Serviços
            var tituloServicos = new Paragraph("SERVIÇOS")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetMarginBottom(10);
            document.Add(tituloServicos);

            var dadosServicos = new Paragraph($"• Serviços do Mês ({inicioMes:MM/yyyy}): {servicosMes:C}")
                .SetFont(font)
                .SetFontSize(12)
                .SetMarginBottom(20);
            document.Add(dadosServicos);

            // Alertas
            if (produtosEstoqueBaixo > 0)
            {
                var alertas = new Paragraph("ALERTAS")
                    .SetFont(boldFont)
                    .SetFontSize(14)
                    .SetFontColor(ColorConstants.RED)
                    .SetMarginBottom(10);
                document.Add(alertas);

                var textoAlerta = new Paragraph($"⚠️ Atenção: {produtosEstoqueBaixo} produto(s) com estoque baixo!")
                    .SetFont(font)
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.RED)
                    .SetBackgroundColor(ColorConstants.YELLOW)
                    .SetPadding(10)
                    .SetMarginBottom(20);
                document.Add(textoAlerta);
            }

            // Rodapé
            var rodape = new Paragraph($"Relatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(font)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFixedPosition(40, 30, 500);
            document.Add(rodape);

            document.Close();

            return File(stream.ToArray(), "application/pdf", $"relatorio_dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
    }
}
