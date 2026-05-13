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
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
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
