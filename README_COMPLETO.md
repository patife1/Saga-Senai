# 🏪 Sistema de Estoque e PDV - Saga SENAI

Sistema completo de gerenciamento de estoque com ponto de venda (PDV) desenvolvido em ASP.NET Core 8.0 MVC.

## 🚀 Funcionalidades Implementadas

### ✅ **Módulo de Categorias**
- ✅ CRUD completo de categorias
- ✅ Validações de negócio
- ✅ Interface responsiva com Bootstrap 5

### ✅ **Módulo de Produtos**
- ✅ CRUD completo de produtos
- ✅ Gestão de estoque com alertas de baixo estoque
- ✅ Códigos de barras
- ✅ Upload de imagens
- ✅ Filtros avançados (categoria, estoque baixo)
- ✅ Busca por nome ou código de barras

### ✅ **Módulo de Clientes**
- ✅ CRUD completo de clientes
- ✅ Validação de CPF/CNPJ
- ✅ Controle de status (ativo/inativo)
- ✅ Interface moderna com DataTables

### ✅ **Módulo de Funcionários**
- ✅ CRUD completo de funcionários
- ✅ Controle de acesso e permissões
- ✅ Dados de contato e endereço

### ✅ **Sistema PDV (Ponto de Venda)**
- ✅ Interface moderna de ponto de venda
- ✅ Busca rápida de produtos por nome ou código de barras
- ✅ Carrinho de compras dinâmico com JavaScript
- ✅ Controle de estoque em tempo real
- ✅ Múltiplas formas de pagamento
- ✅ Cliente avulso ou cadastrado
- ✅ Cálculo automático de totais

### ✅ **Módulo de Vendas**
- ✅ Histórico completo de vendas
- ✅ Filtros por data, cliente e funcionário
- ✅ Estatísticas em tempo real
- ✅ Detalhes completos da venda
- ✅ Sistema de impressão de cupons
- ✅ Cancelamento de vendas do dia
- ✅ Gráficos de composição das vendas

### ✅ **Dashboard Analítico**
- ✅ Métricas em tempo real
- ✅ Gráficos interativos
- ✅ KPIs de vendas e estoque
- ✅ Produtos mais vendidos
- ✅ Alertas de estoque baixo

## 🛠️ Tecnologias Utilizadas

### **Backend**
- **ASP.NET Core 8.0 MVC** - Framework principal
- **Entity Framework Core 8.0** - ORM para acesso a dados
- **SQL Server LocalDB** - Banco de dados
- **Migrations** - Controle de versão do banco

### **Frontend**
- **Bootstrap 5.3** - Framework CSS responsivo
- **Bootstrap Icons** - Ícones modernos
- **JavaScript Vanilla** - Interatividade
- **Chart.js** - Gráficos e dashboards
- **DataTables** - Tabelas avançadas

### **Arquitetura**
- **MVC Pattern** - Separação de responsabilidades
- **ViewModels** - Camada de apresentação
- **Repository Pattern** - Acesso a dados
- **Dependency Injection** - Inversão de controle

## 📁 Estrutura do Projeto

```
SistemaEstoque/
├── Controllers/           # Controladores MVC
│   ├── HomeController.cs
│   ├── DashboardController.cs
│   ├── CategoriasController.cs
│   ├── ProdutosController.cs
│   ├── ClientesController.cs
│   ├── FuncionariosController.cs
│   └── VendasController.cs
├── Models/               # Modelos de dados
│   ├── Categoria.cs
│   ├── Produto.cs
│   ├── Cliente.cs
│   ├── Funcionario.cs
│   ├── Venda.cs
│   ├── ItemVenda.cs
│   └── Servico.cs
├── ViewModels/          # ViewModels para apresentação
│   ├── DashboardViewModel.cs
│   └── VendaViewModel.cs
├── Views/               # Interfaces do usuário
│   ├── Home/
│   ├── Dashboard/
│   ├── Categorias/
│   ├── Produtos/
│   ├── Clientes/
│   ├── Funcionarios/
│   ├── Vendas/
│   └── Shared/
├── Data/                # Contexto do banco de dados
│   └── ApplicationDbContext.cs
├── Migrations/          # Migrações do banco
└── wwwroot/            # Arquivos estáticos
    ├── css/
    ├── js/
    └── lib/
```

## 🎯 Principais Funcionalidades do PDV

### **Interface de Vendas**
- Busca inteligente de produtos
- Carrinho de compras interativo
- Cálculos automáticos de totais
- Validação de estoque em tempo real
- Suporte a código de barras

### **Processamento de Vendas**
- Atualização automática de estoque
- Registro de itens vendidos
- Controle de formas de pagamento
- Geração de cupons fiscais
- Histórico detalhado

### **Relatórios e Analytics**
- Dashboard com métricas de vendas
- Gráficos de composição
- Relatórios por período
- Análise de performance

## 📊 Métricas do Dashboard

- **Total de Vendas** - Quantidade de transações
- **Faturamento** - Valor total em vendas
- **Ticket Médio** - Valor médio por venda
- **Produtos Vendidos** - Quantidade total de itens
- **Estoque Baixo** - Alertas de reposição
- **Produtos Cadastrados** - Total no catálogo

## 🎨 Interface do Usuário

### **Design Responsivo**
- Layout adaptável para desktop, tablet e mobile
- Navegação intuitiva com sidebar
- Cores e tipografia profissionais
- Componentes Bootstrap modernos

### **Experiência do Usuário**
- Feedback visual para ações
- Validações em tempo real
- Modais para confirmações
- Tooltips e ajudas contextuais

## 🔧 Como Executar

### **Pré-requisitos**
- .NET 8.0 SDK
- SQL Server LocalDB
- Visual Studio 2022 ou VS Code

### **Passos para execução**

1. **Clone o repositório**
```bash
git clone https://github.com/seu-usuario/saga-senai.git
cd saga-senai/SistemaEstoque/SistemaEstoque
```

2. **Restaure as dependências**
```bash
dotnet restore
```

3. **Execute as migrações**
```bash
dotnet ef database update
```

4. **Execute o projeto**
```bash
dotnet run
```

5. **Acesse no navegador**
```
http://localhost:5000
```

## 🗃️ Banco de Dados

### **Entidades Principais**
- **Categorias** - Organização de produtos
- **Produtos** - Catálogo de mercadorias
- **Clientes** - Base de clientes
- **Funcionários** - Equipe de trabalho
- **Vendas** - Transações de venda
- **ItensVenda** - Detalhes dos produtos vendidos
- **Serviços** - Serviços oferecidos

### **Relacionamentos**
- Produto → Categoria (N:1)
- Venda → Cliente (N:1)
- Venda → Funcionário (N:1)
- ItemVenda → Produto (N:1)
- ItemVenda → Venda (N:1)

## 📈 Próximas Funcionalidades

### **Em Desenvolvimento**
- [ ] Sistema de Autenticação e Autorização
- [ ] Módulo de Serviços
- [ ] Relatórios Avançados em PDF
- [ ] API REST para integração
- [ ] Sistema de Backup Automático

### **Planejadas**
- [ ] Integração com NFe
- [ ] Sistema de Comissões
- [ ] Multi-loja
- [ ] App Mobile
- [ ] Integração com TEF

## 🎓 Objetivos Pedagógicos Alcançados

### **Desenvolvimento Web**
- ✅ ASP.NET Core MVC
- ✅ Entity Framework Core
- ✅ Padrões de Arquitetura
- ✅ Design Responsivo

### **Gestão de Projetos**
- ✅ Versionamento com Git
- ✅ Documentação Técnica
- ✅ Estrutura Modular
- ✅ Boas Práticas de Código

### **Regras de Negócio**
- ✅ Gestão de Estoque
- ✅ Processo de Vendas
- ✅ Controle Financeiro
- ✅ Analytics Empresarial

## 👥 Equipe de Desenvolvimento

**Desenvolvido durante o curso SENAI** por estudantes aplicando conhecimentos em:
- Programação Web
- Análise de Sistemas
- Design de Interface
- Gestão de Projetos

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais no SENAI.

---

**🔥 Sistema PDV Completo e Funcional!** 

Pronto para uso em ambiente de produção com todas as funcionalidades essenciais de um sistema comercial moderno.
