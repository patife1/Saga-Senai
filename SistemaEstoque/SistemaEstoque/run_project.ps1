# Script para configurar e executar o Sistema de Estoque
Write-Host "======================================"
Write-Host "Configurando Sistema de Estoque"
Write-Host "======================================"

# Definir diretório do projeto
$projectDir = "c:\Users\patife\Downloads\Saga-Senai\SistemaEstoque\SistemaEstoque"

# Ir para o diretório do projeto
Set-Location $projectDir
Write-Host "Diretório atual: $(Get-Location)"

# Verificar versão do .NET
Write-Host "`nVerificando versão do .NET..."
dotnet --version

# Restaurar pacotes
Write-Host "`nRestaurando pacotes..."
dotnet restore

# Compilar projeto
Write-Host "`nCompilando projeto..."
$buildResult = dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilação!" -ForegroundColor Red
    Read-Host "Pressione Enter para continuar..."
    exit
}

# Remover banco antigo se existir
Write-Host "`nRemoção de arquivos antigos..."
if (Test-Path "SistemaEstoque.db") {
    Remove-Item "SistemaEstoque.db" -Force
    Write-Host "Banco antigo removido."
}

# Remover migrations antigas
if (Test-Path "Migrations") {
    Remove-Item "Migrations\*" -Recurse -Force
    Write-Host "Migrations antigas removidas."
}

# Criar nova migration
Write-Host "`nCriando migration..."
dotnet ef migrations add InitialCreate

# Aplicar migration
Write-Host "`nAplicando migration ao banco..."
dotnet ef database update

# Verificar se o banco foi criado
if (Test-Path "SistemaEstoque.db") {
    Write-Host "Banco SQLite criado com sucesso!" -ForegroundColor Green
} else {
    Write-Host "Erro: Banco não foi criado!" -ForegroundColor Red
    Read-Host "Pressione Enter para continuar..."
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
