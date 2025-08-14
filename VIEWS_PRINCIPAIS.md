# 🎨 Views Principais - Sistema de Estoque

## 📁 Estrutura das Views

### 1. Views/Shared/_Layout.cshtml
```html
<!DOCTYPE html>
<html lang="pt-br">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Sistema de Estoque</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css">
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-dark bg-primary border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand fw-bold" asp-area="" asp-controller="Home" asp-action="Index">
                    <i class="bi bi-boxes me-2"></i>Sistema de Estoque
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Home" asp-action="Index">
                                <i class="bi bi-house-door me-1"></i>Dashboard
                            </a>
                        </li>
                        <li class="nav-item dropdown">
                            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                                <i class="bi bi-box me-1"></i>Estoque
                            </a>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item" asp-controller="Estoque" asp-action="Index">
                                    <i class="bi bi-list me-1"></i>Lista de Produtos</a></li>
                                <li><a class="dropdown-item" asp-controller="Estoque" asp-action="Create">
                                    <i class="bi bi-plus-circle me-1"></i>Novo Produto</a></li>
                                <li><a class="dropdown-item" asp-controller="Estoque" asp-action="EstoqueBaixo">
                                    <i class="bi bi-exclamation-triangle me-1"></i>Estoque Baixo</a></li>
                                <li><hr class="dropdown-divider"></li>
                                <li><a class="dropdown-item" asp-controller="Categorias" asp-action="Index">
                                    <i class="bi bi-tags me-1"></i>Categorias</a></li>
                            </ul>
                        </li>
                        <li class="nav-item dropdown">
                            <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                                <i class="bi bi-cart me-1"></i>Vendas
                            </a>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item" asp-controller="Vendas" asp-action="PDV">
                                    <i class="bi bi-credit-card me-1"></i>PDV - Ponto de Venda</a></li>
                                <li><a class="dropdown-item" asp-controller="Vendas" asp-action="Index">
                                    <i class="bi bi-list me-1"></i>Histórico de Vendas</a></li>
                            </ul>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Servicos" asp-action="Index">
                                <i class="bi bi-tools me-1"></i>Serviços
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Clientes" asp-action="Index">
                                <i class="bi bi-person-lines-fill me-1"></i>Clientes
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Funcionarios" asp-action="Index">
                                <i class="bi bi-people me-1"></i>Funcionários
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Relatorios" asp-action="Index">
                                <i class="bi bi-graph-up me-1"></i>Relatórios
                            </a>
                        </li>
                    </ul>
                    <partial name="_LoginPartial" />
                </div>
            </div>
        </nav>
    </header>

    <div class="container-fluid">
        <!-- Alertas -->
        @if (TempData["Success"] != null)
        {
            <div class="alert alert-success alert-dismissible fade show" role="alert">
                <i class="bi bi-check-circle me-2"></i>@TempData["Success"]
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        }
        @if (TempData["Error"] != null)
        {
            <div class="alert alert-danger alert-dismissible fade show" role="alert">
                <i class="bi bi-exclamation-triangle me-2"></i>@TempData["Error"]
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        }
        @if (TempData["Warning"] != null)
        {
            <div class="alert alert-warning alert-dismissible fade show" role="alert">
                <i class="bi bi-exclamation-triangle me-2"></i>@TempData["Warning"]
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        }

        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted mt-5">
        <div class="container">
            <div class="row">
                <div class="col-md-6">
                    &copy; 2024 - Sistema de Estoque - SENAI
                </div>
                <div class="col-md-6 text-end">
                    <small>Desenvolvido para manutenção de eletrodomésticos</small>
                </div>
            </div>
        </div>
    </footer>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

### 2. Views/Home/Index.cshtml (Dashboard)
```html
@model SistemaEstoque.Models.ViewModels.DashboardViewModel
@{
    ViewData["Title"] = "Dashboard";
}

<div class="row mb-4">
    <div class="col-12">
        <h2><i class="bi bi-speedometer2 me-2"></i>Dashboard - Visão Geral</h2>
        <p class="text-muted">Acompanhe os principais indicadores da sua empresa</p>
    </div>
</div>

<!-- Cards de Métricas -->
<div class="row mb-4">
    <div class="col-xl-3 col-md-6 mb-3">
        <div class="card border-start border-primary border-4 h-100">
            <div class="card-body">
                <div class="d-flex align-items-center">
                    <div class="flex-grow-1">
                        <div class="small fw-bold text-primary text-uppercase mb-1">Produtos</div>
                        <div class="h5 mb-0">@Model.TotalProdutos</div>
                    </div>
                    <div class="fa-2x text-gray-300">
                        <i class="bi bi-box text-primary"></i>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-xl-3 col-md-6 mb-3">
        <div class="card border-start border-warning border-4 h-100">
            <div class="card-body">
                <div class="d-flex align-items-center">
                    <div class="flex-grow-1">
                        <div class="small fw-bold text-warning text-uppercase mb-1">Estoque Baixo</div>
                        <div class="h5 mb-0">@Model.ProdutosEstoqueBaixo</div>
                    </div>
                    <div class="fa-2x text-gray-300">
                        <i class="bi bi-exclamation-triangle text-warning"></i>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-xl-3 col-md-6 mb-3">
        <div class="card border-start border-success border-4 h-100">
            <div class="card-body">
                <div class="d-flex align-items-center">
                    <div class="flex-grow-1">
                        <div class="small fw-bold text-success text-uppercase mb-1">Vendas Hoje</div>
                        <div class="h5 mb-0">@Model.VendasHoje.ToString("C")</div>
                    </div>
                    <div class="fa-2x text-gray-300">
                        <i class="bi bi-currency-dollar text-success"></i>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-xl-3 col-md-6 mb-3">
        <div class="card border-start border-info border-4 h-100">
            <div class="card-body">
                <div class="d-flex align-items-center">
                    <div class="flex-grow-1">
                        <div class="small fw-bold text-info text-uppercase mb-1">Vendas do Mês</div>
                        <div class="h5 mb-0">@Model.VendasMes.ToString("C")</div>
                    </div>
                    <div class="fa-2x text-gray-300">
                        <i class="bi bi-graph-up text-info"></i>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Gráficos e Relatórios -->
<div class="row mb-4">
    <div class="col-xl-8 col-lg-7">
        <div class="card h-100">
            <div class="card-header">
                <i class="bi bi-bar-chart me-1"></i>Vendas dos Últimos 7 Dias
            </div>
            <div class="card-body">
                <canvas id="vendasChart" width="100%" height="40"></canvas>
            </div>
        </div>
    </div>
    <div class="col-xl-4 col-lg-5">
        <div class="card h-100">
            <div class="card-header">
                <i class="bi bi-pie-chart me-1"></i>Resumo Financeiro do Mês
            </div>
            <div class="card-body">
                <div class="d-flex align-items-center justify-content-between mb-3">
                    <div>Vendas Total</div>
                    <div class="fw-bold text-success">@Model.VendasMes.ToString("C")</div>
                </div>
                <div class="d-flex align-items-center justify-content-between mb-3">
                    <div>Lucro Total</div>
                    <div class="fw-bold text-primary">@Model.LucroMes.ToString("C")</div>
                </div>
                <div class="d-flex align-items-center justify-content-between">
                    <div>Margem</div>
                    <div class="fw-bold">
                        @(Model.VendasMes > 0 ? ((Model.LucroMes / Model.VendasMes) * 100).ToString("F1") + "%" : "0%")
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Tabelas de Informações -->
<div class="row">
    <div class="col-xl-6 col-lg-6 mb-4">
        <div class="card h-100">
            <div class="card-header">
                <i class="bi bi-clock-history me-1"></i>Últimas Vendas
            </div>
            <div class="card-body">
                @if (Model.UltimasVendas.Any())
                {
                    <div class="table-responsive">
                        <table class="table table-sm">
                            <thead>
                                <tr>
                                    <th>Data</th>
                                    <th>Cliente</th>
                                    <th>Valor</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var venda in Model.UltimasVendas)
                                {
                                    <tr>
                                        <td>@venda.DataVenda.ToString("dd/MM HH:mm")</td>
                                        <td>@(venda.Cliente?.Nome ?? "Cliente Avulso")</td>
                                        <td class="text-success fw-bold">@venda.ValorTotal.ToString("C")</td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                }
                else
                {
                    <p class="text-muted mb-0">Nenhuma venda registrada.</p>
                }
                <div class="text-end mt-2">
                    <a asp-controller="Vendas" asp-action="Index" class="btn btn-outline-primary btn-sm">
                        Ver todas <i class="bi bi-arrow-right"></i>
                    </a>
                </div>
            </div>
        </div>
    </div>

    <div class="col-xl-6 col-lg-6 mb-4">
        <div class="card h-100">
            <div class="card-header">
                <i class="bi bi-calendar-check me-1"></i>Serviços Agendados
            </div>
            <div class="card-body">
                @if (Model.ServicosAgendados.Any())
                {
                    <div class="table-responsive">
                        <table class="table table-sm">
                            <thead>
                                <tr>
                                    <th>Data</th>
                                    <th>Cliente</th>
                                    <th>Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var servico in Model.ServicosAgendados)
                                {
                                    <tr>
                                        <td>@servico.DataServico.ToString("dd/MM HH:mm")</td>
                                        <td>@servico.Cliente.Nome</td>
                                        <td>
                                            <span class="badge bg-@servico.StatusClass">@servico.Status</span>
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                }
                else
                {
                    <p class="text-muted mb-0">Nenhum serviço agendado.</p>
                }
                <div class="text-end mt-2">
                    <a asp-controller="Servicos" asp-action="Index" class="btn btn-outline-primary btn-sm">
                        Ver todos <i class="bi bi-arrow-right"></i>
                    </a>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script>
        // Gráfico de Vendas
        var ctx = document.getElementById('vendasChart').getContext('2d');
        var vendasChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [@Html.Raw(string.Join(",", Model.VendasPorDia.Select(v => "'" + v.Data + "'")))],
                datasets: [{
                    label: 'Vendas (R$)',
                    data: [@Html.Raw(string.Join(",", Model.VendasPorDia.Select(v => v.Valor.ToString("F2").Replace(",", "."))))],
                    borderColor: 'rgb(13, 110, 253)',
                    backgroundColor: 'rgba(13, 110, 253, 0.1)',
                    tension: 0.1,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return 'R$ ' + value.toLocaleString('pt-BR');
                            }
                        }
                    }
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return 'Vendas: R$ ' + context.parsed.y.toLocaleString('pt-BR');
                            }
                        }
                    }
                }
            }
        });

        // Auto-refresh a cada 5 minutos
        setTimeout(function() {
            location.reload();
        }, 300000);
    </script>
}
```

### 3. Views/Estoque/Index.cshtml
```html
@model IEnumerable<SistemaEstoque.Models.Produto>
@{
    ViewData["Title"] = "Estoque";
}

<div class="row mb-4">
    <div class="col-md-6">
        <h2><i class="bi bi-box me-2"></i>Controle de Estoque</h2>
    </div>
    <div class="col-md-6 text-end">
        <a asp-action="Create" class="btn btn-primary">
            <i class="bi bi-plus-circle me-1"></i>Novo Produto
        </a>
        <a asp-action="EstoqueBaixo" class="btn btn-warning">
            <i class="bi bi-exclamation-triangle me-1"></i>Estoque Baixo
        </a>
    </div>
</div>

<!-- Filtros -->
<div class="card mb-4">
    <div class="card-body">
        <form method="get">
            <div class="row">
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Pesquisar:</label>
                        <input name="searchString" value="@ViewBag.SearchString" class="form-control" 
                               placeholder="Nome, descrição ou código de barras..." />
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                        <label>Categoria:</label>
                        <select name="categoriaId" class="form-select">
                            <option value="">Todas</option>
                            @foreach (var categoria in ViewBag.Categorias as SelectList)
                            {
                                <option value="@categoria.Value" selected="@categoria.Selected">@categoria.Text</option>
                            }
                        </select>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                        <label>Filtro:</label>
                        <div class="form-check">
                            <input name="estoqueBaixo" type="checkbox" class="form-check-input" 
                                   value="true" @(ViewBag.EstoqueBaixo == true ? "checked" : "") />
                            <label class="form-check-label">Apenas estoque baixo</label>
                        </div>
                    </div>
                </div>
                <div class="col-md-2 d-flex align-items-end">
                    <button type="submit" class="btn btn-outline-primary w-100">
                        <i class="bi bi-search me-1"></i>Filtrar
                    </button>
                </div>
            </div>
        </form>
    </div>
</div>

<!-- Tabela de Produtos -->
<div class="card">
    <div class="card-body">
        @if (Model.Any())
        {
            <div class="table-responsive">
                <table class="table table-striped table-hover">
                    <thead class="table-dark">
                        <tr>
                            <th>Produto</th>
                            <th>Categoria</th>
                            <th>Estoque</th>
                            <th>Preço Venda</th>
                            <th>Valor Total</th>
                            <th class="text-center">Status</th>
                            <th class="text-center">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var item in Model)
                        {
                            <tr class="@(item.EstoqueBaixo ? "table-warning" : "")">
                                <td>
                                    <div>
                                        <strong>@item.Nome</strong>
                                        @if (!string.IsNullOrEmpty(item.CodigoBarras))
                                        {
                                            <br><small class="text-muted">Código: @item.CodigoBarras</small>
                                        }
                                    </div>
                                </td>
                                <td>@item.Categoria.Nome</td>
                                <td>
                                    <span class="@(item.EstoqueBaixo ? "text-danger fw-bold" : "")">
                                        @item.QuantidadeEstoque
                                    </span>
                                    @if (item.EstoqueBaixo)
                                    {
                                        <i class="bi bi-exclamation-triangle text-warning ms-1" title="Estoque baixo"></i>
                                    }
                                    <br><small class="text-muted">Mín: @item.EstoqueMinimo</small>
                                </td>
                                <td>@item.PrecoVenda.ToString("C")</td>
                                <td>@item.ValorTotalEstoque.ToString("C")</td>
                                <td class="text-center">
                                    @if (item.Ativo)
                                    {
                                        <span class="badge bg-success">Ativo</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-secondary">Inativo</span>
                                    }
                                </td>
                                <td class="text-center">
                                    <div class="btn-group btn-group-sm">
                                        <a asp-action="Details" asp-route-id="@item.Id" 
                                           class="btn btn-outline-info" title="Detalhes">
                                            <i class="bi bi-eye"></i>
                                        </a>
                                        <a asp-action="Edit" asp-route-id="@item.Id" 
                                           class="btn btn-outline-primary" title="Editar">
                                            <i class="bi bi-pencil"></i>
                                        </a>
                                        <button class="btn btn-outline-warning" 
                                                onclick="ajustarEstoque(@item.Id, '@item.Nome', @item.QuantidadeEstoque)" 
                                                title="Ajustar Estoque">
                                            <i class="bi bi-arrow-up-down"></i>
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        }
        else
        {
            <div class="text-center py-5">
                <i class="bi bi-inbox display-1 text-muted"></i>
                <h4 class="text-muted mt-3">Nenhum produto encontrado</h4>
                <p class="text-muted">Tente ajustar os filtros ou adicione um novo produto.</p>
                <a asp-action="Create" class="btn btn-primary">
                    <i class="bi bi-plus-circle me-1"></i>Adicionar Primeiro Produto
                </a>
            </div>
        }
    </div>
</div>

<!-- Modal para Ajustar Estoque -->
<div class="modal fade" id="ajustarEstoqueModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Ajustar Estoque</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <form id="ajustarEstoqueForm">
                    <input type="hidden" id="produtoId" />
                    <div class="mb-3">
                        <label class="form-label">Produto:</label>
                        <p id="nomeProduto" class="fw-bold"></p>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Estoque Atual:</label>
                        <p id="estoqueAtual" class="fw-bold text-primary"></p>
                    </div>
                    <div class="mb-3">
                        <label for="novaQuantidade" class="form-label">Nova Quantidade:</label>
                        <input type="number" class="form-control" id="novaQuantidade" min="0" required />
                    </div>
                    <div class="mb-3">
                        <label for="motivo" class="form-label">Motivo:</label>
                        <textarea class="form-control" id="motivo" rows="3" 
                                  placeholder="Descreva o motivo do ajuste..."></textarea>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" class="btn btn-primary" onclick="confirmarAjuste()">
                    <i class="bi bi-check-circle me-1"></i>Confirmar Ajuste
                </button>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script>
        function ajustarEstoque(id, nome, estoqueAtual) {
            document.getElementById('produtoId').value = id;
            document.getElementById('nomeProduto').textContent = nome;
            document.getElementById('estoqueAtual').textContent = estoqueAtual;
            document.getElementById('novaQuantidade').value = estoqueAtual;
            document.getElementById('motivo').value = '';
            
            var modal = new bootstrap.Modal(document.getElementById('ajustarEstoqueModal'));
            modal.show();
        }

        function confirmarAjuste() {
            var id = document.getElementById('produtoId').value;
            var quantidade = document.getElementById('novaQuantidade').value;
            var motivo = document.getElementById('motivo').value;

            fetch('@Url.Action("AjustarEstoque", "Estoque")', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: `id=${id}&quantidade=${quantidade}&motivo=${encodeURIComponent(motivo)}`
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    location.reload();
                } else {
                    alert('Erro: ' + data.message);
                }
            })
            .catch(error => {
                console.error('Erro:', error);
                alert('Erro ao ajustar estoque');
            });
        }
    </script>
}
```

### 4. Views/Vendas/PDV.cshtml
```html
@model SistemaEstoque.Models.ViewModels.VendaViewModel
@{
    ViewData["Title"] = "PDV - Ponto de Venda";
}

<div class="row mb-4">
    <div class="col-md-8">
        <h2><i class="bi bi-credit-card me-2"></i>PDV - Ponto de Venda</h2>
    </div>
    <div class="col-md-4 text-end">
        <div class="d-flex justify-content-end align-items-center">
            <span class="me-3">Data: <strong>@DateTime.Now.ToString("dd/MM/yyyy HH:mm")</strong></span>
        </div>
    </div>
</div>

<div class="row">
    <!-- Área de Produtos -->
    <div class="col-md-8">
        <div class="card h-100">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0"><i class="bi bi-search me-1"></i>Buscar Produtos</h5>
            </div>
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-md-6">
                        <input type="text" id="buscaProduto" class="form-control" 
                               placeholder="Digite o nome ou código de barras..." />
                    </div>
                    <div class="col-md-6">
                        <select id="filtroCategoria" class="form-select">
                            <option value="">Todas as categorias</option>
                            <!-- Categorias serão carregadas via JavaScript -->
                        </select>
                    </div>
                </div>

                <div id="listaProdutos" class="row">
                    <!-- Produtos serão carregados aqui -->
                </div>
            </div>
        </div>
    </div>

    <!-- Carrinho de Compras -->
    <div class="col-md-4">
        <div class="card h-100">
            <div class="card-header bg-success text-white">
                <h5 class="mb-0"><i class="bi bi-cart me-1"></i>Carrinho de Compras</h5>
            </div>
            <div class="card-body">
                <form asp-action="Create" method="post" id="vendaForm">
                    <!-- Informações da Venda -->
                    <div class="mb-3">
                        <label class="form-label">Cliente (Opcional):</label>
                        <select asp-for="ClienteId" class="form-select">
                            <option value="">Cliente Avulso</option>
                            @foreach (var cliente in Model.Clientes)
                            {
                                <option value="@cliente.Id">@cliente.Nome</option>
                            }
                        </select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Funcionário:</label>
                        <select asp-for="FuncionarioId" class="form-select" required>
                            <option value="">Selecione...</option>
                            @foreach (var funcionario in Model.Funcionarios)
                            {
                                <option value="@funcionario.Id">@funcionario.Nome</option>
                            }
                        </select>
                    </div>

                    <!-- Itens do Carrinho -->
                    <div id="itensCarrinho" class="mb-3">
                        <h6>Itens:</h6>
                        <div id="listaItens">
                            <p class="text-muted text-center">Carrinho vazio</p>
                        </div>
                    </div>

                    <!-- Total -->
                    <div class="border-top pt-3 mb-3">
                        <div class="d-flex justify-content-between">
                            <strong>Total:</strong>
                            <strong id="valorTotal" class="text-success">R$ 0,00</strong>
                        </div>
                    </div>

                    <!-- Forma de Pagamento -->
                    <div class="mb-3">
                        <label class="form-label">Forma de Pagamento:</label>
                        <select asp-for="FormaPagamento" class="form-select" required>
                            <option value="">Selecione...</option>
                            <option value="Dinheiro">Dinheiro</option>
                            <option value="Cartão Crédito">Cartão de Crédito</option>
                            <option value="Cartão Débito">Cartão de Débito</option>
                            <option value="PIX">PIX</option>
                            <option value="Transferência">Transferência</option>
                        </select>
                    </div>

                    <!-- Observações -->
                    <div class="mb-3">
                        <label class="form-label">Observações:</label>
                        <textarea asp-for="Observacoes" class="form-control" rows="2"></textarea>
                    </div>

                    <!-- Botões -->
                    <div class="d-grid gap-2">
                        <button type="button" id="btnFinalizar" class="btn btn-success btn-lg" disabled>
                            <i class="bi bi-check-circle me-1"></i>Finalizar Venda
                        </button>
                        <button type="button" id="btnLimpar" class="btn btn-outline-secondary">
                            <i class="bi bi-arrow-clockwise me-1"></i>Limpar Carrinho
                        </button>
                    </div>

                    <input type="hidden" id="itensJson" name="ItensJson" />
                </form>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script>
        let carrinho = [];
        let produtos = @Html.Raw(Json.Serialize(Model.Produtos.Select(p => new { 
            id = p.Id, 
            nome = p.Nome, 
            preco = p.PrecoVenda, 
            estoque = p.QuantidadeEstoque,
            categoria = p.Categoria.Nome
        })));

        $(document).ready(function() {
            carregarProdutos();
            $('#buscaProduto').on('input', filtrarProdutos);
            $('#filtroCategoria').on('change', filtrarProdutos);
            $('#btnFinalizar').on('click', finalizarVenda);
            $('#btnLimpar').on('click', limparCarrinho);
        });

        function carregarProdutos() {
            let html = '';
            produtos.forEach(produto => {
                html += `
                    <div class="col-md-6 col-lg-4 mb-2">
                        <div class="card produto-card" onclick="adicionarAoCarrinho(${produto.id})">
                            <div class="card-body p-2">
                                <h6 class="card-title mb-1">${produto.nome}</h6>
                                <p class="card-text mb-1">
                                    <strong class="text-success">${formatarMoeda(produto.preco)}</strong><br>
                                    <small class="text-muted">Estoque: ${produto.estoque}</small>
                                </p>
                            </div>
                        </div>
                    </div>
                `;
            });
            $('#listaProdutos').html(html);
        }

        function filtrarProdutos() {
            let busca = $('#buscaProduto').val().toLowerCase();
            let categoria = $('#filtroCategoria').val();

            let produtosFiltrados = produtos.filter(p => {
                let matchBusca = busca === '' || p.nome.toLowerCase().includes(busca);
                let matchCategoria = categoria === '' || p.categoria === categoria;
                return matchBusca && matchCategoria;
            });

            let html = '';
            produtosFiltrados.forEach(produto => {
                html += `
                    <div class="col-md-6 col-lg-4 mb-2">
                        <div class="card produto-card" onclick="adicionarAoCarrinho(${produto.id})">
                            <div class="card-body p-2">
                                <h6 class="card-title mb-1">${produto.nome}</h6>
                                <p class="card-text mb-1">
                                    <strong class="text-success">${formatarMoeda(produto.preco)}</strong><br>
                                    <small class="text-muted">Estoque: ${produto.estoque}</small>
                                </p>
                            </div>
                        </div>
                    </div>
                `;
            });
            $('#listaProdutos').html(html);
        }

        function adicionarAoCarrinho(produtoId) {
            let produto = produtos.find(p => p.id === produtoId);
            if (!produto) return;

            let itemExistente = carrinho.find(item => item.produtoId === produtoId);
            
            if (itemExistente) {
                if (itemExistente.quantidade < produto.estoque) {
                    itemExistente.quantidade++;
                } else {
                    alert('Estoque insuficiente!');
                    return;
                }
            } else {
                carrinho.push({
                    produtoId: produtoId,
                    nomeProduto: produto.nome,
                    precoUnitario: produto.preco,
                    quantidade: 1
                });
            }

            atualizarCarrinho();
        }

        function removerDoCarrinho(produtoId) {
            carrinho = carrinho.filter(item => item.produtoId !== produtoId);
            atualizarCarrinho();
        }

        function alterarQuantidade(produtoId, novaQuantidade) {
            let produto = produtos.find(p => p.id === produtoId);
            let item = carrinho.find(item => item.produtoId === produtoId);
            
            if (novaQuantidade <= 0) {
                removerDoCarrinho(produtoId);
                return;
            }

            if (novaQuantidade > produto.estoque) {
                alert('Estoque insuficiente!');
                return;
            }

            item.quantidade = novaQuantidade;
            atualizarCarrinho();
        }

        function atualizarCarrinho() {
            let html = '';
            let total = 0;

            if (carrinho.length === 0) {
                html = '<p class="text-muted text-center">Carrinho vazio</p>';
            } else {
                carrinho.forEach(item => {
                    let subtotal = item.quantidade * item.precoUnitario;
                    total += subtotal;

                    html += `
                        <div class="border-bottom mb-2 pb-2">
                            <div class="d-flex justify-content-between align-items-start">
                                <div class="flex-grow-1">
                                    <h6 class="mb-1">${item.nomeProduto}</h6>
                                    <small class="text-muted">${formatarMoeda(item.precoUnitario)} cada</small>
                                </div>
                                <button class="btn btn-sm btn-outline-danger" onclick="removerDoCarrinho(${item.produtoId})">
                                    <i class="bi bi-trash"></i>
                                </button>
                            </div>
                            <div class="d-flex justify-content-between align-items-center mt-1">
                                <div class="btn-group btn-group-sm">
                                    <button class="btn btn-outline-secondary" onclick="alterarQuantidade(${item.produtoId}, ${item.quantidade - 1})">-</button>
                                    <span class="btn btn-outline-secondary">${item.quantidade}</span>
                                    <button class="btn btn-outline-secondary" onclick="alterarQuantidade(${item.produtoId}, ${item.quantidade + 1})">+</button>
                                </div>
                                <strong class="text-success">${formatarMoeda(subtotal)}</strong>
                            </div>
                        </div>
                    `;
                });
            }

            $('#listaItens').html(html);
            $('#valorTotal').text(formatarMoeda(total));
            $('#btnFinalizar').prop('disabled', carrinho.length === 0);
            $('#itensJson').val(JSON.stringify(carrinho));
        }

        function limparCarrinho() {
            carrinho = [];
            atualizarCarrinho();
        }

        function finalizarVenda() {
            if (carrinho.length === 0) {
                alert('Adicione pelo menos um produto ao carrinho!');
                return;
            }

            if (!$('#FuncionarioId').val()) {
                alert('Selecione um funcionário!');
                return;
            }

            if (!$('#FormaPagamento').val()) {
                alert('Selecione uma forma de pagamento!');
                return;
            }

            $('#vendaForm').submit();
        }

        function formatarMoeda(valor) {
            return new Intl.NumberFormat('pt-BR', {
                style: 'currency',
                currency: 'BRL'
            }).format(valor);
        }
    </script>

    <style>
        .produto-card {
            cursor: pointer;
            transition: all 0.2s;
        }

        .produto-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }

        #itensCarrinho {
            max-height: 300px;
            overflow-y: auto;
        }
    </style>
}
```

## 📋 Lista de Views Criadas

- [x] **_Layout.cshtml** - Layout principal responsivo
- [x] **Home/Index.cshtml** - Dashboard com métricas e gráficos
- [x] **Estoque/Index.cshtml** - Lista de produtos com filtros
- [x] **Vendas/PDV.cshtml** - Ponto de Venda interativo

## 🎯 Próximas Views a Criar

1. **Estoque/Create.cshtml** - Formulário de novo produto
2. **Estoque/Edit.cshtml** - Edição de produto
3. **Vendas/Index.cshtml** - Histórico de vendas
4. **Clientes/Index.cshtml** - Lista de clientes
5. **Funcionarios/Index.cshtml** - Lista de funcionários
6. **Servicos/Index.cshtml** - Lista de serviços

## 🎨 Recursos de UI Implementados

- ✅ **Bootstrap 5.3** - Framework CSS responsivo
- ✅ **Bootstrap Icons** - Ícones modernos
- ✅ **Charts.js** - Gráficos interativos
- ✅ **Alertas Dinâmicos** - Feedback visual
- ✅ **Modais** - Popups para ações
- ✅ **Filtros Avançados** - Busca e filtros
- ✅ **PDV Interativo** - Carrinho de compras funcional

Essas views fornecem uma interface moderna e funcional para o sistema de estoque, com foco na usabilidade e responsividade.
