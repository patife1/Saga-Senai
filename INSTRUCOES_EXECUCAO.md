# Como executar o projeto Sistema de Estoque

## Problemas identificados e corrigidos:

1. **Versão do .NET**: Atualizado de .NET 8.0 para .NET 9.0 (compatível com sua instalação)
2. **Banco de dados**: Mudado de SQL Server LocalDB para SQLite (mais fácil para desenvolvimento)
3. **Warnings de referências nulas**: Corrigidos nos controladores
4. **Relacionamentos EF Core**: Corrigidos no modelo ItemVenda

## Arquivos atualizados:

- `SistemaEstoque.csproj` - Atualizado para .NET 9.0 e adicionado SQLite
- `appsettings.json` - String de conexão alterada para SQLite
- `Program.cs` - Configurado para usar SQLite em vez de SQL Server
- `Controllers/ClientesController.cs` - Correção de warnings de referências nulas
- `Controllers/ProdutosController.cs` - Correção de warnings de referências nulas
- `Models/ItemVenda.cs` - Removidas anotações conflitantes de ForeignKey

## Para executar o projeto:

### Opção 1 - Usando o arquivo batch (mais fácil):
1. Execute o arquivo `setup_and_run.bat` clicando duas vezes nele
2. Aguarde a compilação e execução automática

### Opção 2 - Comandos manuais:
1. Abra o PowerShell como administrador
2. Navegue até a pasta do projeto:
   ```
   cd "c:\Users\patife\Downloads\Saga-Senai\SistemaEstoque\SistemaEstoque"
   ```
3. Execute os comandos na sequência:
   ```
   dotnet restore
   dotnet build
   dotnet ef migrations add InitialCreateSQLite
   dotnet ef database update
   dotnet run
   ```

## Acesso ao sistema:

Após executar, o sistema estará disponível em:
- **URL**: http://localhost:5187 (ou a porta mostrada no terminal)
- **Banco de dados**: Um arquivo `SistemaEstoque.db` será criado na pasta do projeto

## Funcionalidades disponíveis:

- Dashboard com métricas
- Gerenciamento de Categorias
- Gerenciamento de Produtos
- Gerenciamento de Clientes
- Gerenciamento de Funcionários
- Sistema de Vendas
- Relatórios
- Sistema de autenticação

## Usuários padrão criados automaticamente:

O sistema criará automaticamente usuários de teste ao iniciar pela primeira vez.

## Observações:

- O arquivo de banco SQLite (`SistemaEstoque.db`) será criado automaticamente
- Todos os warnings de compilação foram corrigidos
- O projeto agora é compatível com .NET 9.0

Se encontrar algum problema, verifique se:
1. O .NET 9.0 SDK está instalado
2. Você está executando no diretório correto do projeto
3. Não há outro processo usando a porta 5187
