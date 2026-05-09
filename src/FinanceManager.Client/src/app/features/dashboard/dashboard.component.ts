import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
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
    BaseChartDirective,
  ],
  template: `
    <div class="page-container">
      <h1 class="page-title">Dashboard</h1>

      @if (loading()) {
        <div class="loading-center">
          <mat-spinner diameter="48" />
        </div>
      } @else {
        <!-- Summary Cards -->
        <div class="summary-cards">
          <mat-card class="summary-card">
            <mat-card-content>
              <div class="card-icon" style="background: #e8f5e9;">
                <mat-icon style="color: #2e7d32;">trending_up</mat-icon>
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
              <div class="card-icon" style="background: #ffebee;">
                <mat-icon style="color: #c62828;">trending_down</mat-icon>
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
              <div class="card-icon" style="background: #fce4ec;">
                <mat-icon style="color: #ad1457;">warning_amber</mat-icon>
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
              @if (barChartData.datasets[0].data.length > 0) {
                <canvas baseChart
                  [data]="barChartData"
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
              @if (pieChartData.datasets[0].data.length > 0) {
                <canvas baseChart
                  [data]="pieChartData"
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

    .charts-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 16px;
      margin-bottom: 24px;

      @media (max-width: 900px) {
        grid-template-columns: 1fr;
      }
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
        &.icon-revenue { background: #e8f5e9; mat-icon { color: #2e7d32; } }
        &.icon-expense { background: #ffebee; mat-icon { color: #c62828; } }
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

  loading = signal(true);
  transactions = signal<Transaction[]>([]);
  categories = signal<Category[]>([]);

  totalRevenue = computed(() =>
    this.transactions()
      .filter(t => t.type === 'Revenue' && t.status === 'Paid')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  totalExpense = computed(() =>
    this.transactions()
      .filter(t => t.type === 'Expense' && t.status === 'Paid')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  balance = computed(() => this.totalRevenue() - this.totalExpense());

  pendingCount = computed(() =>
    this.transactions().filter(t => t.status === 'Pending').length
  );

  overdueCount = computed(() =>
    this.transactions().filter(t => t.isOverdue && t.status === 'Pending').length
  );

  recentTransactions = computed(() =>
    [...this.transactions()]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 5)
  );

  // Bar chart: revenue vs expense per category
  barChartData: ChartData<'bar'> = {
    labels: [],
    datasets: [
      { label: 'Receitas', data: [], backgroundColor: '#66bb6a' },
      { label: 'Despesas', data: [], backgroundColor: '#ef5350' },
    ],
  };

  barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'top' } },
    scales: {
      y: { beginAtZero: true, ticks: { callback: (v) => `R$ ${Number(v).toFixed(0)}` } },
    },
  };

  // Doughnut: status distribution
  pieChartData: ChartData<'doughnut'> = {
    labels: ['Pendente', 'Pago', 'Cancelado'],
    datasets: [{
      data: [],
      backgroundColor: ['#ff9800', '#4caf50', '#9e9e9e'],
    }],
  };

  pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'bottom' } },
  };

  ngOnInit(): void {
    forkJoin({
      transactions: this.transactionService.getAll(),
      categories: this.categoryService.getAll(),
    }).subscribe({
      next: ({ transactions, categories }) => {
        this.transactions.set(transactions);
        this.categories.set(categories);
        this.buildCharts(transactions, categories);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = {
      Pending: 'Pendente',
      Paid: 'Pago',
      Cancelled: 'Cancelado',
    };
    return map[status] ?? status;
  }

  private buildCharts(transactions: Transaction[], categories: Category[]): void {
    // Bar chart by category
    const catLabels = categories.map(c => c.name);
    const revenues = categories.map(c =>
      transactions
        .filter(t => t.categoryName === c.name && t.type === 'Revenue' && t.status === 'Paid')
        .reduce((s, t) => s + t.amount, 0)
    );
    const expenses = categories.map(c =>
      transactions
        .filter(t => t.categoryName === c.name && t.type === 'Expense' && t.status === 'Paid')
        .reduce((s, t) => s + t.amount, 0)
    );

    this.barChartData = {
      labels: catLabels,
      datasets: [
        { label: 'Receitas', data: revenues, backgroundColor: '#66bb6a' },
        { label: 'Despesas', data: expenses, backgroundColor: '#ef5350' },
      ],
    };

    // Pie chart by status
    const pending = transactions.filter(t => t.status === 'Pending').length;
    const paid = transactions.filter(t => t.status === 'Paid').length;
    const cancelled = transactions.filter(t => t.status === 'Cancelled').length;

    this.pieChartData = {
      labels: ['Pendente', 'Pago', 'Cancelado'],
      datasets: [{
        data: [pending, paid, cancelled],
        backgroundColor: ['#ff9800', '#4caf50', '#9e9e9e'],
      }],
    };
  }
}
