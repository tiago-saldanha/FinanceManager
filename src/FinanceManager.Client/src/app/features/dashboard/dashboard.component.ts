import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ThemeService } from '../../core/services/theme.service';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData } from 'chart.js';
import { Chart, registerables } from 'chart.js';
import { forkJoin } from 'rxjs';

import { TransactionService } from '../../core/services/transaction.service';
import { CategoryService } from '../../core/services/category.service';
import { Transaction } from '../../core/models/transaction.model';
import { Category } from '../../core/models/category.model';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    DatePipe,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatDividerModule,
    MatButtonToggleModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatInputModule,
    BaseChartDirective,
  ],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1 class="page-title">Dashboard</h1>
        <div class="page-filters">
          <mat-form-field appearance="outline" class="period-select" subscriptSizing="dynamic">
            <mat-label>Período</mat-label>
            <mat-select [value]="periodMode()" (valueChange)="setPeriod($event)">
              <mat-option value="thisMonth">Este mês</mat-option>
              <mat-option value="lastMonth">Mês passado</mat-option>
              <mat-option value="last30Days">Últimos 30 dias</mat-option>
              <mat-option value="thisYear">Este ano</mat-option>
              <mat-option value="all">Tudo</mat-option>
              <mat-option value="custom">Personalizado</mat-option>
            </mat-select>
          </mat-form-field>

          @if (periodMode() === 'custom') {
            <mat-form-field appearance="outline" class="date-range" subscriptSizing="dynamic">
              <mat-label>Data Inicial</mat-label>
              <input matInput
                     [matDatepicker]="startPicker"
                     [value]="customStartDate()"
                     (dateChange)="customStartDate.set($event.value)" />
              <mat-datepicker-toggle matIconSuffix [for]="startPicker" />
              <mat-datepicker #startPicker />
            </mat-form-field>

            <mat-form-field appearance="outline" class="date-range" subscriptSizing="dynamic">
              <mat-label>Data Final</mat-label>
              <input matInput
                     [matDatepicker]="endPicker"
                     [value]="customEndDate()"
                     [min]="customStartDate()"
                     (dateChange)="customEndDate.set($event.value)" />
              <mat-datepicker-toggle matIconSuffix [for]="endPicker" />
              <mat-datepicker #endPicker />
            </mat-form-field>
          }

          <mat-button-toggle-group
            [value]="filterMode()"
            (change)="setFilter($event.value)"
            aria-label="Filtrar transações"
            class="filter-toggle">
            <mat-button-toggle value="all">Todas</mat-button-toggle>
            <mat-button-toggle value="paid">Pagas</mat-button-toggle>
            <mat-button-toggle value="unpaid">Pendentes</mat-button-toggle>
          </mat-button-toggle-group>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-center">
          <mat-spinner diameter="48" />
        </div>
      } @else {
        <!-- Summary Cards -->
        <div class="summary-cards">
          <mat-card class="summary-card">
            <mat-card-content>
              <div class="card-icon" style="background: var(--color-revenue-bg);">
                <mat-icon style="color: var(--color-revenue-text);">trending_up</mat-icon>
              </div>
              <div class="card-info">
                <div class="card-label">Total Receitas</div>
                <div class="card-value text-revenue">
                  {{ totalRevenue() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                </div>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="summary-card">
            <mat-card-content>
              <div class="card-icon" style="background: var(--color-expense-bg);">
                <mat-icon style="color: var(--color-expense-text);">trending_down</mat-icon>
              </div>
              <div class="card-info">
                <div class="card-label">Total Despesas</div>
                <div class="card-value text-expense">
                  {{ totalExpense() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                </div>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="summary-card">
            <mat-card-content>
              <div class="card-icon" style="background: #e3f2fd;">
                <mat-icon style="color: #1565c0;">account_balance</mat-icon>
              </div>
              <div class="card-info">
                <div class="card-label">Saldo</div>
                <div class="card-value" [class.text-revenue]="balance() >= 0" [class.text-expense]="balance() < 0">
                  {{ balance() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                </div>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="summary-card">
            <mat-card-content>
              <div class="card-icon" style="background: #fff8e1;">
                <mat-icon style="color: #e65100;">pending_actions</mat-icon>
              </div>
              <div class="card-info">
                <div class="card-label">Pendentes</div>
                <div class="card-value text-pending">{{ pendingCount() }}</div>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="summary-card">
            <mat-card-content>
              <div class="card-icon" style="background: var(--color-expense-bg);">
                <mat-icon style="color: var(--color-expense-text);">warning_amber</mat-icon>
              </div>
              <div class="card-info">
                <div class="card-label">Em Atraso</div>
                <div class="card-value text-overdue">{{ overdueCount() }}</div>
              </div>
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Charts Row -->
        <div class="charts-grid">
          <!-- Revenue vs Expense by Category (Bar) -->
          <mat-card class="chart-card">
            <mat-card-header>
              <mat-card-title>Receitas vs Despesas por Categoria</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              @if (barChartData().datasets[0].data.length > 0) {
                <canvas baseChart
                  [data]="barChartData()"
                  [options]="barChartOptions"
                  type="bar">
                </canvas>
              } @else {
                <div class="empty-chart">
                  <mat-icon>bar_chart</mat-icon>
                  <p>Sem dados para exibir</p>
                </div>
              }
            </mat-card-content>
          </mat-card>

          <!-- Transaction status (Pie) -->
          <mat-card class="chart-card chart-card--small">
            <mat-card-header>
              <mat-card-title>Status das Transações</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              @if (pieChartData().datasets[0].data.length > 0) {
                <canvas baseChart
                  [data]="pieChartData()"
                  [options]="pieChartOptions"
                  type="doughnut">
                </canvas>
              } @else {
                <div class="empty-chart">
                  <mat-icon>pie_chart</mat-icon>
                  <p>Sem dados para exibir</p>
                </div>
              }
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Category Pies Row -->
        <div class="charts-grid charts-grid--two">
          <mat-card class="chart-card">
            <mat-card-header>
              <mat-card-title>Receitas por Categoria</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              @if (revenueByCategoryChart().datasets[0].data.length > 0) {
                <canvas baseChart
                  [data]="revenueByCategoryChart()"
                  [options]="categoryPieOptions"
                  type="pie">
                </canvas>
              } @else {
                <div class="empty-chart">
                  <mat-icon>pie_chart</mat-icon>
                  <p>Sem receitas no período</p>
                </div>
              }
            </mat-card-content>
          </mat-card>

          <mat-card class="chart-card">
            <mat-card-header>
              <mat-card-title>Despesas por Categoria</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              @if (expenseByCategoryChart().datasets[0].data.length > 0) {
                <canvas baseChart
                  [data]="expenseByCategoryChart()"
                  [options]="categoryPieOptions"
                  type="pie">
                </canvas>
              } @else {
                <div class="empty-chart">
                  <mat-icon>pie_chart</mat-icon>
                  <p>Sem despesas no período</p>
                </div>
              }
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Recent Transactions -->
        <mat-card class="recent-card">
          <mat-card-header>
            <mat-card-title>Transações Recentes</mat-card-title>
            <div class="card-header-action">
              <a mat-button color="primary" routerLink="/transactions">Ver todas</a>
            </div>
          </mat-card-header>
          <mat-card-content>
            @if (recentTransactions().length === 0) {
              <p class="empty-text">Nenhuma transação cadastrada.</p>
            } @else {
              <div class="transaction-list">
                @for (tx of recentTransactions(); track tx.id) {
                  <div class="transaction-item">
                    <div class="tx-icon" [class.icon-revenue]="tx.type === 'Revenue'" [class.icon-expense]="tx.type === 'Expense'">
                      <mat-icon>{{ tx.type === 'Revenue' ? 'arrow_upward' : 'arrow_downward' }}</mat-icon>
                    </div>
                    <div class="tx-info">
                      <div class="tx-description">{{ tx.description }}</div>
                      <div class="tx-meta">{{ tx.categoryName }} · {{ tx.dueDate | date:'dd/MM/yyyy' }}</div>
                    </div>
                    <div class="tx-right">
                      <div class="tx-amount" [class.text-revenue]="tx.type === 'Revenue'" [class.text-expense]="tx.type === 'Expense'">
                        {{ (tx.type === 'Expense' ? -tx.amount : tx.amount) | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                      </div>
                      <mat-chip [class]="'chip-' + tx.status.toLowerCase()">
                        {{ statusLabel(tx.status) }}
                      </mat-chip>
                    </div>
                  </div>
                  <mat-divider />
                }
              </div>
            }
          </mat-card-content>
        </mat-card>
      }
    </div>
  `,
  styles: [`
    .loading-center {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 300px;
    }

    .page-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      flex-wrap: wrap;
      gap: 12px;
      margin-bottom: 16px;

      .page-title {
        margin: 0;
      }
    }

    .page-filters {
      display: flex;
      align-items: center;
      gap: 12px;
      flex-wrap: wrap;
    }

    .period-select {
      width: 200px;
    }

    .date-range {
      width: 170px;
    }

    .filter-toggle {
      align-self: center;
    }

    .charts-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 16px;
      margin-bottom: 24px;

      @media (max-width: 900px) {
        grid-template-columns: 1fr;
      }
    }

    .charts-grid--two {
      grid-template-columns: 1fr 1fr;
    }

    .chart-card {
      mat-card-header {
        margin-bottom: 16px;
      }
      canvas {
        max-height: 300px;
      }
    }

    .empty-chart {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 200px;
      color: #bdbdbd;
      mat-icon { font-size: 48px; width: 48px; height: 48px; }
      p { margin-top: 8px; }
    }

    .recent-card {
      mat-card-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 8px;
      }
      .card-header-action {
        margin-left: auto;
      }
    }

    .transaction-list { width: 100%; }
    .transaction-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 0;

      .tx-icon {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        &.icon-revenue { background: var(--color-revenue-bg); mat-icon { color: var(--color-revenue-text); } }
        &.icon-expense { background: var(--color-expense-bg); mat-icon { color: var(--color-expense-text); } }
      }

      .tx-info {
        flex: 1;
        .tx-description { font-weight: 500; font-size: 14px; }
        .tx-meta { font-size: 12px; color: #757575; margin-top: 2px; }
      }

      .tx-right {
        text-align: right;
        .tx-amount { font-weight: 600; font-size: 15px; margin-bottom: 4px; }
      }
    }

    .empty-text {
      text-align: center;
      color: #9e9e9e;
      padding: 24px 0;
    }
  `],
})
export class DashboardComponent implements OnInit {
  private readonly transactionService = inject(TransactionService);
  private readonly categoryService = inject(CategoryService);
  private readonly themeService = inject(ThemeService);

  private cssVar(name: string): string {
    return getComputedStyle(document.body).getPropertyValue(name).trim();
  }

  loading = signal(true);
  transactions = signal<Transaction[]>([]);
  categories = signal<Category[]>([]);

  // Filtro de status: todas / pagas / pendentes.
  filterMode = signal<'all' | 'paid' | 'unpaid'>('all');

  // Filtro de período. Padrão: este mês.
  periodMode = signal<'thisMonth' | 'lastMonth' | 'last30Days' | 'thisYear' | 'all' | 'custom'>('thisMonth');

  // Datas usadas quando o período é 'custom'. Enquanto não preenchidas o
  // filtro fica desligado (mesmo comportamento de "Tudo").
  customStartDate = signal<Date | null>(null);
  customEndDate = signal<Date | null>(null);

  // Transações dentro do período selecionado (independe do filtro de status).
  // O filtro é aplicado no `dueDate` da transação.
  periodFiltered = computed(() => {
    const range = this.resolvePeriodRange();
    const all = this.transactions();
    if (!range) return all;
    const { start, end } = range;
    return all.filter(t => {
      const d = new Date(t.dueDate);
      return d >= start && d < end;
    });
  });

  // Resolve o intervalo atual considerando tanto os modos predefinidos
  // quanto o modo 'custom' (que depende dos signals customStart/customEnd).
  private resolvePeriodRange(): { start: Date; end: Date } | null {
    const mode = this.periodMode();
    if (mode === 'custom') {
      const start = this.customStartDate();
      const end = this.customEndDate();
      if (!start || !end) return null;
      // O fim do intervalo é exclusivo, então somamos 1 dia para
      // incluir transações que vencem no próprio dia "Data Final".
      const inclusiveEnd = new Date(end);
      inclusiveEnd.setHours(0, 0, 0, 0);
      inclusiveEnd.setDate(inclusiveEnd.getDate() + 1);
      const normalizedStart = new Date(start);
      normalizedStart.setHours(0, 0, 0, 0);
      return { start: normalizedStart, end: inclusiveEnd };
    }
    return this.getPeriodRange(mode);
  }

  // Transações depois de aplicar período + filtro de status.
  // 'all' inclui Paid e Pending (Cancelled é deixado de fora por padrão).
  filteredTransactions = computed(() => {
    const all = this.periodFiltered();
    switch (this.filterMode()) {
      case 'paid':
        return all.filter(t => t.status === 'Paid');
      case 'unpaid':
        return all.filter(t => t.status === 'Pending');
      default:
        return all.filter(t => t.status !== 'Cancelled');
    }
  });

  totalRevenue = computed(() =>
    this.filteredTransactions()
      .filter(t => t.type === 'Revenue')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  totalExpense = computed(() =>
    this.filteredTransactions()
      .filter(t => t.type === 'Expense')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  balance = computed(() => this.totalRevenue() - this.totalExpense());

  // Contadores de pendentes/atraso ignoram o filtro de status (sempre são sobre
  // pendentes), mas respeitam o período selecionado.
  pendingCount = computed(() =>
    this.periodFiltered().filter(t => t.status === 'Pending').length
  );

  overdueCount = computed(() =>
    this.periodFiltered().filter(t => t.isOverdue && t.status === 'Pending').length
  );

  recentTransactions = computed(() =>
    [...this.filteredTransactions()]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 5)
  );

  // Bar chart: receitas vs despesas por categoria, respeitando o filtro
  barChartData = computed<ChartData<'bar'>>(() => {
    void this.themeService.current();
    const filtered = this.filteredTransactions();
    const cats = this.categories();
    const labels = cats.map(c => c.name);
    const revenues = cats.map(c =>
      filtered
        .filter(t => t.categoryName === c.name && t.type === 'Revenue')
        .reduce((s, t) => s + t.amount, 0)
    );
    const expenses = cats.map(c =>
      filtered
        .filter(t => t.categoryName === c.name && t.type === 'Expense')
        .reduce((s, t) => s + t.amount, 0)
    );
    return {
      labels,
      datasets: [
        { label: 'Receitas', data: revenues, backgroundColor: this.cssVar('--color-revenue') },
        { label: 'Despesas', data: expenses, backgroundColor: this.cssVar('--color-expense') },
      ],
    };
  });

  barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'top' } },
    scales: {
      y: { beginAtZero: true, ticks: { callback: (v) => `R$ ${Number(v).toFixed(0)}` } },
    },
  };

  // Doughnut: distribuição de status dentro do período selecionado.
  pieChartData = computed<ChartData<'doughnut'>>(() => {
    void this.themeService.current();
    const all = this.periodFiltered();
    const pending = all.filter(t => t.status === 'Pending').length;
    const paid = all.filter(t => t.status === 'Paid').length;
    const cancelled = all.filter(t => t.status === 'Cancelled').length;
    return {
      labels: ['Pendente', 'Pago', 'Cancelado'],
      datasets: [{
        data: [pending, paid, cancelled],
        backgroundColor: ['#ff9800', this.cssVar('--color-revenue'), '#9e9e9e'],
      }],
    };
  });

  pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'bottom' } },
  };

  // Paletas separadas para receitas (verdes/azuis) e despesas (vermelhos/laranjas).
  private static readonly REVENUE_PALETTE = [
    '#1D9E75', '#0F6E56', '#5e35b1', '#1e88e5', '#00897b',
    '#7cb342', '#039be5', '#3949ab', '#00acc1', '#558b2f',
  ];
  private static readonly EXPENSE_PALETTE = [
    '#D85A30', '#993C1D', '#8e24aa', '#d81b60', '#f4511e',
    '#993C1D', '#D85A30', '#BA7517', '#6d4c41', '#bf360c',
  ];

  // Pizza: receitas por categoria (respeita período + filtro de status).
  revenueByCategoryChart = computed<ChartData<'pie'>>(() => {
    void this.themeService.current();
    const palette = [this.cssVar('--color-revenue'), ...DashboardComponent.REVENUE_PALETTE.slice(1)];
    return this.buildCategoryPie('Revenue', palette);
  });

  // Pizza: despesas por categoria (respeita período + filtro de status).
  expenseByCategoryChart = computed<ChartData<'pie'>>(() => {
    void this.themeService.current();
    const palette = [this.cssVar('--color-expense'), ...DashboardComponent.EXPENSE_PALETTE.slice(1)];
    return this.buildCategoryPie('Expense', palette);
  });

  categoryPieOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' },
      tooltip: {
        callbacks: {
          label: (ctx) => {
            const value = Number(ctx.parsed) || 0;
            const dataset = ctx.dataset.data as number[];
            const total = dataset.reduce((s, v) => s + (Number(v) || 0), 0);
            const pct = total > 0 ? ((value / total) * 100).toFixed(1) : '0';
            const formatted = value.toLocaleString('pt-BR', {
              style: 'currency',
              currency: 'BRL',
            });
            return `${ctx.label}: ${formatted} (${pct}%)`;
          },
        },
      },
    },
  };

  private buildCategoryPie(
    type: 'Revenue' | 'Expense',
    palette: readonly string[]
  ): ChartData<'pie'> {
    const filtered = this.filteredTransactions().filter(t => t.type === type);
    const totalsByCategory = new Map<string, number>();
    for (const t of filtered) {
      const key = t.categoryName ?? 'Sem categoria';
      totalsByCategory.set(key, (totalsByCategory.get(key) ?? 0) + t.amount);
    }
    // Mantém somente categorias com valor > 0 e ordena do maior para o menor
    // (a fatia maior fica em destaque na pizza).
    const entries = [...totalsByCategory.entries()]
      .filter(([, v]) => v > 0)
      .sort((a, b) => b[1] - a[1]);

    const labels = entries.map(([k]) => k);
    const data = entries.map(([, v]) => v);
    const backgroundColor = entries.map((_, i) => palette[i % palette.length]);

    return {
      labels,
      datasets: [{ data, backgroundColor }],
    };
  }

  ngOnInit(): void {
    forkJoin({
      transactions: this.transactionService.getAll(),
      categories: this.categoryService.getAll(),
    }).subscribe({
      next: ({ transactions, categories }) => {
        this.transactions.set(transactions);
        this.categories.set(categories);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  setFilter(value: 'all' | 'paid' | 'unpaid'): void {
    this.filterMode.set(value);
  }

  setPeriod(value: 'thisMonth' | 'lastMonth' | 'last30Days' | 'thisYear' | 'all' | 'custom'): void {
    this.periodMode.set(value);
    // Ao entrar no modo personalizado pela primeira vez, pré-preenche o
    // intervalo com o mês atual para o usuário ter um ponto de partida.
    if (value === 'custom' && (!this.customStartDate() || !this.customEndDate())) {
      const now = new Date();
      const start = new Date(now.getFullYear(), now.getMonth(), 1);
      const end = new Date(now.getFullYear(), now.getMonth() + 1, 0);
      this.customStartDate.set(start);
      this.customEndDate.set(end);
    }
  }

  // Retorna o intervalo [start, end) correspondente ao período selecionado,
  // ou null para "tudo" (sem filtro de data).
  private getPeriodRange(
    mode: 'thisMonth' | 'lastMonth' | 'last30Days' | 'thisYear' | 'all' | 'custom'
  ): { start: Date; end: Date } | null {
    const now = new Date();
    switch (mode) {
      case 'thisMonth': {
        const start = new Date(now.getFullYear(), now.getMonth(), 1);
        const end = new Date(now.getFullYear(), now.getMonth() + 1, 1);
        return { start, end };
      }
      case 'lastMonth': {
        const start = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        const end = new Date(now.getFullYear(), now.getMonth(), 1);
        return { start, end };
      }
      case 'last30Days': {
        const start = new Date(now);
        start.setDate(start.getDate() - 30);
        start.setHours(0, 0, 0, 0);
        const end = new Date(now);
        end.setDate(end.getDate() + 1);
        end.setHours(0, 0, 0, 0);
        return { start, end };
      }
      case 'thisYear': {
        const start = new Date(now.getFullYear(), 0, 1);
        const end = new Date(now.getFullYear() + 1, 0, 1);
        return { start, end };
      }
      case 'all':
      case 'custom':
      default:
        return null;
    }
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = {
      Pending: 'Pendente',
      Paid: 'Pago',
      Cancelled: 'Cancelado',
    };
    return map[status] ?? status;
  }
}
