# 🏪 Distribuidora - Sistema de Gestão

Sistema completo de gestão de distribuidora com controle de estoque, vendas e análise de lucros. Aplicação web moderna com interface premium e dashboard em tempo real.

![Dashboard](https://img.shields.io/badge/Status-Produção-success)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
![License](https://img.shields.io/badge/License-MIT-blue)

## 🎯 Funcionalidades

### 📊 Dashboard em Tempo Real
- Total de produtos cadastrados
- Lucro total acumulado
- Quantidade total em estoque
- Valor total investido em estoque

### 📦 Gestão de Produtos
- ➕ **Cadastrar** novos produtos
- ✏️ **Editar** informações (nome, preços)
- 🗑️ **Excluir** produtos
- 🔍 **Listar** todos os produtos com detalhes
- ⚠️ **Alertas** visuais para estoque baixo (≤5 unidades)

### 💰 Controle de Transações
- **Entrada de Estoque** (compras)
- **Saída de Estoque** (vendas)
- Cálculo automático de lucro por venda
- Atualização automática do lucro total
- Validação de estoque disponível

### 📈 Análises e Relatórios
- Margem de lucro por produto
- Indicadores visuais de estoque (baixo/médio/alto)
- Histórico completo de transações
- Formatação de valores em Real (R$)

## 🚀 Tecnologias Utilizadas

### Backend
- **ASP.NET Core 9.0** - Framework web
- **Npgsql** - Driver PostgreSQL
- **Swagger** - Documentação da API

### Frontend
- **HTML5** - Estrutura semântica
- **CSS3** - Design moderno com gradientes e animações
- **JavaScript (Vanilla)** - Lógica e integração com API
- **Google Fonts (Inter)** - Tipografia premium

### Banco de Dados
- **PostgreSQL** (Neon Database) - Banco de dados em nuvem

## 📋 Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [ASP.NET Core Runtime 9.0](https://dotnet.microsoft.com/download)
- PostgreSQL (ou acesso ao Neon Database)

## 🔧 Instalação

### 1. Clone o repositório
```bash
git clone <url-do-repositorio>
cd Distribuidora
```

### 2. Configure a connection string
Edite o arquivo `appsettings.json` e configure a string de conexão:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=seu-host;Port=5432;Database=seu-db;Username=usuario;Password=senha;SSL Mode=Require;"
  }
}
```

### 3. Restaure as dependências
```bash
dotnet restore
```

### 4. Execute a aplicação
```bash
dotnet run
```

A aplicação estará disponível em: **http://localhost:5000**

## 🎨 Interface

### Design Premium
- 🌑 **Tema Dark** com gradientes vibrantes
- ✨ **Animações suaves** em hover e transições
- 📱 **Layout responsivo** para desktop e mobile
- 🎯 **Badges coloridos** para indicadores visuais
- 🔔 **Modais elegantes** para formulários

### Paleta de Cores
- **Primary**: Gradiente roxo-azul (#6366f1 → #8b5cf6)
- **Success**: Verde (#10b981)
- **Warning**: Amarelo (#f59e0b)
- **Danger**: Vermelho (#ef4444)
- **Background**: Dark (#0f172a com gradiente)

## 📁 Estrutura do Projeto

```
Distribuidora/
├── Controllers/          # Controladores da API
│   ├── ProdutosController.cs
│   ├── TransacoesController.cs
│   └── DashboardController.cs
├── Models/              # Modelos de dados
│   ├── Produto.cs
│   ├── Transacao.cs
│   ├── TipoTransacao.cs
│   └── DadosDistribuidora.cs
├── Repositories/        # Acesso a dados
│   ├── IRepositorioProduto.cs
│   └── RepositorioProduto.cs
├── wwwroot/            # Frontend
│   ├── css/
│   │   └── style.css
│   ├── js/
│   │   └── app.js
│   └── index.html
├── Program.cs          # Configuração da aplicação
└── appsettings.json    # Configurações
```

## 🔌 API Endpoints

### Produtos
- `GET /api/produtos` - Listar todos os produtos
- `GET /api/produtos/{id}` - Obter produto por ID
- `POST /api/produtos` - Criar novo produto
- `PUT /api/produtos/{id}` - Atualizar produto
- `DELETE /api/produtos/{id}` - Excluir produto

### Transações
- `GET /api/transacoes` - Listar todas as transações
- `POST /api/transacoes` - Registrar nova transação

### Dashboard
- `GET /api/dashboard` - Obter estatísticas gerais

## 💡 Exemplos de Uso

### Cadastrar Produto
```json
POST /api/produtos
{
  "nome": "Produto Exemplo",
  "quantidadeEstoque": 100,
  "precoCusto": 10.00,
  "precoVenda": 20.00
}
```

### Registrar Venda
```json
POST /api/transacoes
{
  "produtoId": 1,
  "nomeProduto": "Produto Exemplo",
  "tipo": "Saida",
  "quantidade": 5
}
```

## 🐛 Troubleshooting

### Erro: "dotnet: comando não encontrado"
Instale o .NET SDK seguindo as instruções em: https://dotnet.microsoft.com/download

### Erro: "Failed to determine the https port"
Este é apenas um aviso e não afeta o funcionamento. A aplicação roda em HTTP.

### Erro 400 ao criar transação
Certifique-se de que:
- O produto existe no banco de dados
- O tipo é "Entrada" ou "Saida" (sem acento na API)
- A quantidade é um número válido

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👨‍💻 Autor

Desenvolvido com ❤️ usando ASP.NET Core e design moderno.

## 🎯 Roadmap

- [ ] Autenticação de usuários
- [ ] Relatórios em PDF
- [ ] Gráficos interativos (Chart.js)
- [ ] Exportação de dados (Excel/CSV)
- [ ] API de integração com sistemas externos
- [ ] PWA (Progressive Web App)
- [ ] Notificações push para estoque baixo
- [ ] Multi-idiomas (i18n)

---

⭐ Se este projeto foi útil para você, considere dar uma estrela!