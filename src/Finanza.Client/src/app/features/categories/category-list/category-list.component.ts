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
import { CategoryFormComponent, CategoryFormData } from '../category-form/category-form.component';

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
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.scss',
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

  openEditDialog(cat: Category): void {
    const data: CategoryFormData = { category: cat };
    const ref = this.dialog.open(CategoryFormComponent, {
      width: '480px',
      disableClose: true,
      data,
    });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Categoria atualizada com sucesso!', 'OK', { duration: 3000 });
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
