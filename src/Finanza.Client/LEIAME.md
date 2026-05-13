 Finance Manager â€” Frontend Angular 18

Frontend do sistema de controle financeiro, construÃ­do com Angular 18 + Angular Material.

## Como rodar

### PrÃ©-requisitos
- Node.js 18+ e npm 9+
- Angular CLI 18: `npm install -g @angular/cli@18`

### InstalaÃ§Ã£o

```bash
cd finanza-client
npm install
ng serve
```

A aplicaÃ§Ã£o estarÃ¡ disponÃ­vel em `http://localhost:4200`.

> Certifique-se de que o backend .NET estÃ¡ rodando em `http://localhost:5000` antes de iniciar o frontend.

## Estrutura do projeto

```
src/app/
â”œâ”€â”€ core/
â”‚   â”œâ”€â”€ models/              # Interfaces TypeScript (Transaction, Category)
â”‚   â”œâ”€â”€ services/            # TransactionService, CategoryService
â”‚   â””â”€â”€ interceptors/        # Error interceptor (snackbar de erros)
â”œâ”€â”€ features/
â”‚   â”œâ”€â”€ dashboard/           # Dashboard com cards e grÃ¡ficos
â”‚   â”œâ”€â”€ transactions/
â”‚   â”‚   â”œâ”€â”€ transaction-list/  # Tabela com filtros e aÃ§Ãµes
â”‚   â”‚   â””â”€â”€ transaction-form/  # Dialog de criaÃ§Ã£o
â”‚   â””â”€â”€ categories/
â”‚       â”œâ”€â”€ category-list/     # Grid de cards com totais
â”‚       â””â”€â”€ category-form/     # Dialog de criaÃ§Ã£o
â””â”€â”€ layout/                  # Sidenav + Toolbar
```

## Tecnologias

- **Angular 18** â€” Standalone Components, Signals, View Transitions
- **Angular Material 18** â€” UI components
- **Chart.js + ng2-charts** â€” GrÃ¡ficos do dashboard
- **RxJS** â€” Reatividade e chamadas HTTP

## ConfiguraÃ§Ã£o da API

Edite `src/environments/environment.ts` para apontar para a URL correta do backend:

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // â† ajuste aqui
};
```

## Funcionalidades

- **Dashboard**: Cards de resumo (receitas, despesas, saldo, pendentes, em atraso), grÃ¡fico de barras por categoria e grÃ¡fico de pizza por status das transaÃ§Ãµes
- **TransaÃ§Ãµes**: Listagem com busca, filtro por status e tipo, paginaÃ§Ã£o, aÃ§Ãµes de pagar/cancelar/reabrir e formulÃ¡rio de criaÃ§Ã£o
- **Categorias**: Grid de cards com totais (recebido, gasto, saldo) e barra de progresso de consumo por categoria
