@echo off
cd "c:\Users\patife\Downloads\Saga-Senai\SistemaEstoque\SistemaEstoque"
echo Restaurando pacotes...
dotnet restore
echo.
echo Compilando projeto...
dotnet build
echo.
echo Criando migration...
dotnet ef migrations add InitialCreateSQLite
echo.
echo Aplicando migration...
dotnet ef database update
echo.
echo Executando projeto...
dotnet run
pause
