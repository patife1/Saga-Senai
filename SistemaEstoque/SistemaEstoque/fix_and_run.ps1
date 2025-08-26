# Script para corrigir o banco de dados e executar o projeto
Write-Host "======================================"
Write-Host "Corrigindo Sistema de Estoque"
Write-Host "======================================"

# Ir para o diretório do projeto
Set-Location "c:\Users\patife\Downloads\Saga-Senai\SistemaEstoque\SistemaEstoque"

# Parar qualquer processo em execução
Write-Host "Parando processos existentes..."
Get-Process | Where-Object {$_.ProcessName -eq "SistemaEstoque"} | Stop-Process -Force -ErrorAction SilentlyContinue

# Limpar arquivos antigos
Write-Host "Limpando arquivos antigos..."
Remove-Item "SistemaEstoque.db" -Force -ErrorAction SilentlyContinue
Remove-Item "Migrations\*" -Recurse -Force -ErrorAction SilentlyContinue

# Compilar projeto
Write-Host "Compilando projeto..."
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilação!" -ForegroundColor Red
    Read-Host "Pressione Enter para sair..."
    exit
}

# Criar migration
Write-Host "Criando migration..."
dotnet ef migrations add InitialCreate

# Aplicar migration
Write-Host "Aplicando migration..."
dotnet ef database update

# Verificar se o banco foi criado
if (Test-Path "SistemaEstoque.db") {
    Write-Host "Banco SQLite criado com sucesso!" -ForegroundColor Green
    
    # Verificar se as tabelas foram criadas usando sqlite3 se disponível
    Write-Host "Tentando verificar tabelas no banco..."
    try {
        # Se sqlite3 estiver disponível, verificar as tabelas
        $tables = & sqlite3 SistemaEstoque.db ".tables" 2>$null
        if ($tables) {
            Write-Host "Tabelas encontradas no banco:" -ForegroundColor Green
            Write-Host $tables
        }
    }
    catch {
        Write-Host "SQLite3 não disponível para verificação, mas banco foi criado."
    }
} else {
    Write-Host "Erro: Banco não foi criado!" -ForegroundColor Red
    Read-Host "Pressione Enter para sair..."
    exit
}

Write-Host "`n======================================"
Write-Host "Iniciando o servidor..."
Write-Host "======================================"
Write-Host "O sistema estará disponível em: http://localhost:5187" -ForegroundColor Yellow
Write-Host "Para parar o servidor, pressione Ctrl+C" -ForegroundColor Yellow
Write-Host "`n"

# Executar o projeto
dotnet run
