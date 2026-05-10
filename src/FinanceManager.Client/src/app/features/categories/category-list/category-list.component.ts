import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { startWith, forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CategoryService } from '../../../core/services/category.service';
import { TransactionService } from '../../../core/services/transaction.service';
import { Category } from '../../../core/models/category.model';
import { Transaction } from '../../../core/models/transaction.model';
import { CategoryFormComponent } from '../category-form/category-form.component';

type PeriodMode = 'thisMonth' | 'lastMonth' | 'last30Days' | 'thisYear' | 'all' | 'custom';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTooltipModule,
  ],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1 class="page-title">Categorias</h1>
        <button mat-raised-button color="primary" (click)="openCreateDialog()">
          <mat-icon>add</mat-icon>
          Nova Categoria
        </button>
      </div>

      <!-- Filters -->
      <div class="filters-bar">
        <mat-form-field appearance="outline" subscriptSizing="dynamic" class="search-field">
          <mat-label>Buscar categoria</mat-label>
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchCtrl" placeholder="Nome da categoria..." />
        </mat-form-field>

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

        <span class="filter-count">{{ filteredCategories().length }} categoria(s)</span>
      </div>

      @if (loading()) {
        <div class="loading-center">
          <mat-spinner diameter="48" />
        </div>
      } @else if (filteredCategories().length === 0) {
        <div class="empty-state">
          <mat-icon>category</mat-icon>
          <p>Nenhuma categoria encontrada</p>
          <button mat-raised-button color="primary" (click)="openCreateDialog()">
            Criar primeira categoria
          </button>
        </div>
      } @else {
        <!-- Summary row -->
        <div class="totals-banner">
          <div class="total-item">
            <mat-icon style="color: #2e7d32;">trending_up</mat-icon>
            <div>
              <div class="total-label">Total Recebido</div>
              <div class="total-value text-revenue">
                {{ grandTotalReceived() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
              </div>
            </div>
          </div>
          <mat-divider vertical />
          <div class="total-item">
            <mat-icon style="color: #c62828;">trending_down</mat-icon>
            <div>
              <div class="total-label">Total Gasto</div>
              <div class="total-value text-expense">
                {{ grandTotalSpent() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
              </div>
            </div>
          </div>
          <mat-divider vertical />
          <div class="total-item">
            <mat-icon style="color: #1565c0;">account_balance</mat-icon>
            <div>
              <div class="total-label">Saldo Geral</div>
              <div class="total-value"
                [class.text-revenue]="grandBalance() >= 0"
                [class.text-expense]="grandBalance() < 0">
                {{ grandBalance() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
              </div>
            </div>
          </div>
        </div>

        <!-- Cards grid -->
        <div class="category-grid">
          @for (cat of filteredCategories(); track cat.id) {
            <mat-card class="category-card">
              <mat-card-header>
                <div class="cat-avatar" mat-card-avatar>
                  <mat-icon>label</mat-icon>
                </div>
                <mat-card-title>{{ cat.name }}</mat-card-title>
                @if (cat.description) {
                  <mat-card-subtitle>{{ cat.description }}</mat-card-subtitle>
                }
              </mat-card-header>

              <mat-card-content>
                <div class="cat-totals">
                  <div class="cat-total-item">
                    <div class="ct-label">Recebido</div>
                    <div class="ct-value text-revenue">
                      {{ cat.total.received | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                    </div>
                  </div>
                  <div class="cat-divider"></div>
                  <div class="cat-total-item">
                    <div class="ct-label">Gasto</div>
                    <div class="ct-value text-expense">
                      {{ cat.total.spent | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                    </div>
                  </div>
                  <div class="cat-divider"></div>
                  <div class="cat-total-item">
                    <div class="ct-label">Saldo</div>
                    <div class="ct-value"
                      [class.text-revenue]="cat.total.balance >= 0"
                      [class.text-expense]="cat.total.balance < 0">
                      {{ cat.total.balance | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                    </div>
                  </div>
                </div>

                <!-- Balance progress bar -->
                @if (cat.total.received > 0 || cat.total.spent > 0) {
                  <div class="balance-bar-wrapper" matTooltip="Percentual gasto do total recebido">
                    <div class="balance-bar">
                      <div
                        class="balance-bar-fill"
                        [style.width.%]="spentPercent(cat)"
                        [style.background]="spentPercent(cat) > 90 ? '#f44336' : spentPercent(cat) > 60 ? '#ff9800' : '#4caf50'"
                      ></div>
                    </div>
                    <span class="balance-bar-label">{{ spentPercent(cat) | number:'1.0-0' }}% gasto</span>
                  </div>
                }
              </mat-card-content>
            </mat-card>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 24px;
      .page-title { margin-bottom: 0; }
    }

    .filters-bar {
      display: flex;
      align-items: center;
      gap: 12px;
      flex-wrap: wrap;
      margin-bottom: 20px;
      .filter-count { color: #757575; font-size: 13px; margin-left: auto; }
    }

    .search-field {
      width: 320px;
    }

    .period-select {
      width: 200px;
    }

    .date-range {
      width: 170px;
    }

    .loading-center {
      display: flex;
      justify-content: center;
      padding: 48px;
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 64px;
      color: #9e9e9e;
      mat-icon { font-size: 56px; width: 56px; height: 56px; }
      p { font-size: 16px; }
    }

    .totals-banner {
      display: flex;
      align-items: center;
      gap: 24px;
      background: white;
      border-radius: 8px;
      padding: 16px 24px;
      margin-bottom: 24px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.12);

      .total-item {
        display: flex;
        align-items: center;
        gap: 12px;
        flex: 1;
        mat-icon { font-size: 28px; width: 28px; height: 28px; }
        .total-label { font-size: 12px; color: #757575; margin-bottom: 2px; }
        .total-value { font-size: 18px; font-weight: 600; }
      }

      mat-divider { height: 40px; }
    }

    .category-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 16px;
    }

    .category-card {
      transition: box-shadow 0.2s;
      &:hover { box-shadow: 0 4px 12px rgba(0,0,0,0.15); }

      .cat-avatar {
        background: #d1fae5;
        border-radius: 50%;
        width: 40px;
        height: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
        mat-icon { color: #00695c; }
      }

      .cat-totals {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-top: 16px;
        margin-bottom: 12px;

        .cat-total-item {
          flex: 1;
          text-align: center;
          .ct-label { font-size: 11px; color: #757575; margin-bottom: 4px; text-transform: uppercase; letter-spacing: 0.5px; }
          .ct-value { font-size: 14px; font-weight: 600; }
        }

        .cat-divider {
          width: 1px;
          height: 36px;
          background: #e0e0e0;
        }
      }

      .balance-bar-wrapper {
        display: flex;
        align-items: center;
        gap: 8px;
        margin-top: 4px;

        .balance-bar {
          flex: 1;
          height: 6px;
          background: #e0e0e0;
          border-radius: 3px;
          overflow: hidden;
          .balance-bar-fill {
            height: 100%;
            border-radius: 3px;
            transition: width 0.4s ease;
            max-width: 100%;
          }
        }

        .balance-bar-label {
          font-size: 11px;
          color: #757575;
          white-space: nowrap;
        }
      }
    }

    @media (max-width: 600px) {
      .totals-banner {
        flex-direction: column;
        gap: 12px;
        mat-divider { display: none; }
        .total-item { width: 100%; }
      }
    }
  `],
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);
  private readonly transactionService = inject(TransactionService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  loading = signal(true);
  categories = signal<Category[]>([]);
  transactions = signal<Transaction[]>([]);
  searchCtrl = new FormControl('');

  // Filtro de período (mesmo padrão do Dashboard).
  periodMode = signal<PeriodMode>('thisMonth');
  customStartDate = signal<Date | null>(null);
  customEndDate = signal<Date | null>(null);

  private searchValue = toSignal(
    this.searchCtrl.valueChanges.pipe(startWith('')),
    { initialValue: '' }
  );

  // Transações filtradas pelo período selecionado.
  private periodFilteredTransactions = computed(() => {
    const range = this.resolvePeriodRange();
    const all = this.transactions();
    if (!range) return all;
    const { start, end } = range;
    return all.filter(t => {
      const d = new Date(t.dueDate);
      return d >= start && d < end;
    });
  });

  // Categorias com totais recalculados a partir das transações do período.
  // Mantém a mesma fórmula do backend (received - spent, sem filtrar status).
  private displayCategories = computed<Category[]>(() => {
    const filtered = this.periodFilteredTransactions();
    return this.categories().map(cat => {
      const catTxs = filtered.filter(t => t.categoryName === cat.name);
      const received = catTxs
        .filter(t => t.type === 'Revenue')
        .reduce((s, t) => s + t.amount, 0);
      const spent = catTxs
        .filter(t => t.type === 'Expense')
        .reduce((s, t) => s + t.amount, 0);
      return {
        ...cat,
        total: { received, spent, balance: received - spent },
      };
    });
  });

  filteredCategories = computed(() => {
    const search = (this.searchValue() ?? '').toLowerCase();
    return this.displayCategories().filter(c =>
      !search || c.name.toLowerCase().includes(search)
    );
  });

  grandTotalReceived = computed(() =>
    this.filteredCategories().reduce((s, c) => s + c.total.received, 0)
  );

  grandTotalSpent = computed(() =>
    this.filteredCategories().reduce((s, c) => s + c.total.spent, 0)
  );

  grandBalance = computed(() => this.grandTotalReceived() - this.grandTotalSpent());

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading.set(true);
    forkJoin({
      categories: this.categoryService.getAll(),
      transactions: this.transactionService.getAll(),
    }).subscribe({
      next: ({ categories, transactions }) => {
        this.categories.set(categories);
        this.transactions.set(transactions);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(CategoryFormComponent, {
      width: '480px',
      disableClose: true,
    });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Categoria criada com sucesso!', 'OK', { duration: 3000 });
        this.loadAll();
      }
    });
  }

  spentPercent(cat: Category): number {
    if (cat.total.received === 0) return cat.total.spent > 0 ? 100 : 0;
    return Math.min((cat.total.spent / cat.total.received) * 100, 100);
  }

  setPeriod(value: PeriodMode): void {
    this.periodMode.set(value);
    if (value === 'custom' && (!this.customStartDate() || !this.customEndDate())) {
      const now = new Date();
      const start = new Date(now.getFullYear(), now.getMonth(), 1);
      const end = new Date(now.getFullYear(), now.getMonth() + 1, 0);
      this.customStartDate.set(start);
      this.customEndDate.set(end);
    }
  }

  // Resolve o intervalo [start, end) atual considerando preset ou custom.
  private resolvePeriodRange(): { start: Date; end: Date } | null {
    const mode = this.periodMode();
    if (mode === 'custom') {
      const start = this.customStartDate();
      const end = this.customEndDate();
      if (!start || !end) return null;
      const inclusiveEnd = new Date(end);
      inclusiveEnd.setHours(0, 0, 0, 0);
      inclusiveEnd.setDate(inclusiveEnd.getDate() + 1);
      const normalizedStart = new Date(start);
      normalizedStart.setHours(0, 0, 0, 0);
      return { start: normalizedStart, end: inclusiveEnd };
    }
    return this.getPeriodRange(mode);
  }

  private getPeriodRange(mode: PeriodMode): { start: Date; end: Date } | null {
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
}
