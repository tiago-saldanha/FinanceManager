# Finance Manager — Frontend Angular 18

Frontend do sistema de controle financeiro, construído com Angular 18 + Angular Material.

## Como rodar

### Pré-requisitos
- Node.js 18+ e npm 9+
- Angular CLI 18: `npm install -g @angular/cli@18`

### Instalação

```bash
cd finance-manager-frontend
npm install
ng serve
```

A aplicação estará disponível em `http://localhost:4200`.

> Certifique-se de que o backend .NET está rodando em `http://localhost:5000` antes de iniciar o frontend.

## Estrutura do projeto

```
src/app/
├── core/
│   ├── models/              # Interfaces TypeScript (Transaction, Category)
│   ├── services/            # TransactionService, CategoryService
│   └── interceptors/        # Error interceptor (snackbar de erros)
├── features/
│   ├── dashboard/           # Dashboard com cards e gráficos
│   ├── transactions/
│   │   ├── transaction-list/  # Tabela com filtros e ações
│   │   └── transaction-form/  # Dialog de criação
│   └── categories/
│       ├── category-list/     # Grid de cards com totais
│       └── category-form/     # Dialog de criação
└── layout/                  # Sidenav + Toolbar
```

## Tecnologias

- **Angular 18** — Standalone Components, Signals, View Transitions
- **Angular Material 18** — UI components
- **Chart.js + ng2-charts** — Gráficos do dashboard
- **RxJS** — Reatividade e chamadas HTTP

## Configuração da API

Edite `src/environments/environment.ts` para apontar para a URL correta do backend:

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // ← ajuste aqui
};
```

## Funcionalidades

- **Dashboard**: Cards de resumo (receitas, despesas, saldo, pendentes, em atraso), gráfico de barras por categoria e gráfico de pizza por status das transações
- **Transações**: Listagem com busca, filtro por status e tipo, paginação, ações de pagar/cancelar/reabrir e formulário de criação
- **Categorias**: Grid de cards com totais (recebido, gasto, saldo) e barra de progresso de consumo por categoria
