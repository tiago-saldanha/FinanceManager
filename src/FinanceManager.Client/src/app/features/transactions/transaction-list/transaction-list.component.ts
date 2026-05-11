import { Component, OnInit, OnDestroy, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDatepickerModule } from '@angular/material/datepicker';

import { TransactionService } from '../../../core/services/transaction.service';
import {
  Transaction,
  TransactionStatus,
  TransactionType,
  TRANSACTION_STATUS_LABELS,
  TRANSACTION_TYPE_LABELS,
} from '../../../core/models/transaction.model';
import { combineLatest, debounceTime, distinctUntilChanged, Subject, switchMap, takeUntil, startWith } from 'rxjs';
import { TransactionFormComponent, TransactionFormData } from '../transaction-form/transaction-form.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { PayTransactionDialogComponent } from '../pay-transaction-dialog/pay-transaction-dialog.component';

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    DatePipe,
    ReactiveFormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatMenuModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatSortModule,
    MatPaginatorModule,
    MatDatepickerModule,
    ConfirmDialogComponent,
  ],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1 class="page-title">Transações</h1>
        <button mat-raised-button color="primary" (click)="openCreateDialog()">
          <mat-icon>add</mat-icon>
          Nova Transação
        </button>
      </div>

      <!-- Filter bar -->
      <div class="filter-bar">
        <mat-form-field appearance="outline" subscriptSizing="dynamic" class="search-field">
          <mat-label>Buscar</mat-label>
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchCtrl" placeholder="Descrição ou categoria..." />
        </mat-form-field>

        <mat-form-field appearance="outline" subscriptSizing="dynamic" class="select-field">
          <mat-label>Status</mat-label>
          <mat-select [formControl]="statusCtrl">
            <mat-option value="">Todos</mat-option>
            <mat-option value="Pending">Pendente</mat-option>
            <mat-option value="Paid">Pago</mat-option>
            <mat-option value="Cancelled">Cancelado</mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" subscriptSizing="dynamic" class="select-field">
          <mat-label>Tipo</mat-label>
          <mat-select [formControl]="typeCtrl">
            <mat-option value="">Todos</mat-option>
            <mat-option value="Revenue">Receita</mat-option>
            <mat-option value="Expense">Despesa</mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" subscriptSizing="dynamic" class="date-field">
          <mat-label>Data Inicial</mat-label>
          <input matInput [matDatepicker]="startPicker" [formControl]="startDateCtrl" />
          <mat-datepicker-toggle matIconSuffix [for]="startPicker" />
          <mat-datepicker #startPicker />
        </mat-form-field>

        <mat-form-field appearance="outline" subscriptSizing="dynamic" class="date-field">
          <mat-label>Data Final</mat-label>
          <input matInput [matDatepicker]="endPicker" [formControl]="endDateCtrl" />
          <mat-datepicker-toggle matIconSuffix [for]="endPicker" />
          <mat-datepicker #endPicker />
        </mat-form-field>

        @if (activeFilters()) {
          <button mat-icon-button matTooltip="Limpar filtros" (click)="clearFilters()">
            <mat-icon>filter_alt_off</mat-icon>
          </button>
        }
      </div>

      <!-- Table -->
      <mat-card>
        @if (loading()) {
          <div class="loading-center">
            <mat-spinner diameter="40" />
          </div>
        } @else if (transactions().length === 0) {
          <div class="empty-state">
            <mat-icon>receipt_long</mat-icon>
            <p>Nenhuma transação encontrada</p>
            <button mat-raised-button color="primary" (click)="openCreateDialog()">
              Criar primeira transação
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="paginatedTransactions()" class="mat-elevation-z0">

            <!-- Type Column -->
            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Tipo</th>
              <td mat-cell *matCellDef="let row">
                <div class="type-indicator" [class.type-revenue]="row.type === 'Revenue'" [class.type-expense]="row.type === 'Expense'">
                  <mat-icon>{{ row.type === 'Revenue' ? 'arrow_upward' : 'arrow_downward' }}</mat-icon>
                </div>
              </td>
            </ng-container>

            <!-- Description Column -->
            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Descrição</th>
              <td mat-cell *matCellDef="let row">
                <div class="desc-cell">
                  <span class="desc-text">{{ row.description }}</span>
                  <span class="category-badge">{{ row.categoryName }}</span>
                </div>
              </td>
            </ng-container>

            <!-- Amount Column -->
            <ng-container matColumnDef="amount">
              <th mat-header-cell *matHeaderCellDef>Valor</th>
              <td mat-cell *matCellDef="let row">
                <span [class.text-revenue]="row.type === 'Revenue'" [class.text-expense]="row.type === 'Expense'" style="font-weight: 600;">
                  {{ row.amount | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                </span>
              </td>
            </ng-container>

            <!-- Due Date Column -->
            <ng-container matColumnDef="dueDate">
              <th mat-header-cell *matHeaderCellDef>Vencimento</th>
              <td mat-cell *matCellDef="let row">
                <span [class.text-overdue]="row.isOverdue && row.status === 'Pending'">
                  {{ row.dueDate | date:'dd/MM/yyyy' }}
                  @if (row.isOverdue && row.status === 'Pending') {
                    <mat-icon class="overdue-icon" matTooltip="Em atraso">warning</mat-icon>
                  }
                </span>
              </td>
            </ng-container>

            <!-- Status Column -->
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let row">
                <mat-chip [class]="'chip-' + row.status.toLowerCase()" style="font-size: 12px;">
                  {{ statusLabel(row.status) }}
                </mat-chip>
              </td>
            </ng-container>

            <!-- Actions Column -->
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let row">
                <button mat-icon-button [matMenuTriggerFor]="menu" [matMenuTriggerData]="{row}">
                  <mat-icon>more_vert</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>

          <mat-paginator
            [length]="transactions().length"
            [pageSize]="pageSize"
            [pageSizeOptions]="[10, 25, 50]"
            (page)="onPageChange($event)"
            showFirstLastButtons
          />
        }
      </mat-card>
    </div>

    <!-- Actions Menu -->
    <mat-menu #menu="matMenu">
      <ng-template matMenuContent let-row="row">
        @if (row.status === 'Pending') {
          <button mat-menu-item (click)="openEditDialog(row)">
            <mat-icon>edit</mat-icon>
            <span>Editar</span>
          </button>
          <button mat-menu-item (click)="payTransaction(row)">
            <mat-icon color="primary">payments</mat-icon>
            <span>Marcar como Pago</span>
          </button>
          <button mat-menu-item (click)="cancelTransaction(row)">
            <mat-icon color="warn">cancel</mat-icon>
            <span>Cancelar</span>
          </button>
        }
        @if (row.status === 'Paid' || row.status === 'Cancelled') {
          <button mat-menu-item (click)="reopenTransaction(row)">
            <mat-icon>refresh</mat-icon>
            <span>Reabrir (Pendente)</span>
          </button>
        }
        <mat-divider />
        <button mat-menu-item class="menu-item-danger" (click)="confirmRemove(row)">
          <mat-icon>delete_outline</mat-icon>
          <span>Excluir</span>
        </button>
      </ng-template>
    </mat-menu>
  `,
  styles: [`
    .page-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 24px;
      .page-title { margin-bottom: 0; }
    }

    .search-field { flex: 1; min-width: 180px; }
    .select-field { width: 130px; flex-shrink: 0; }
    .date-field   { width: 148px; flex-shrink: 0; }

    .filter-count {
      margin-left: auto;
      color: #757575;
      font-size: 13px;
      white-space: nowrap;
      flex-shrink: 0;
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
      justify-content: center;
      gap: 12px;
      padding: 48px;
      color: #9e9e9e;
      mat-icon { font-size: 56px; width: 56px; height: 56px; }
      p { font-size: 16px; }
    }

    .type-indicator {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      &.type-revenue { background: var(--color-revenue-bg); mat-icon { color: var(--color-revenue-text); font-size: 20px; width: 20px; height: 20px; line-height: 20px; } }
      &.type-expense { background: var(--color-expense-bg); mat-icon { color: var(--color-expense-text); font-size: 20px; width: 20px; height: 20px; line-height: 20px; } }
    }

    .desc-cell {
      display: flex;
      flex-direction: column;
      .desc-text { font-weight: 500; }
      .category-badge {
        font-size: 11px;
        color: #757575;
        background: #f5f5f5;
        border-radius: 4px;
        padding: 1px 6px;
        margin-top: 2px;
        display: inline-block;
        width: fit-content;
      }
    }

    .overdue-icon {
      font-size: 14px;
      width: 14px;
      height: 14px;
      vertical-align: middle;
      margin-left: 4px;
      color: #f44336;
    }

    table { width: 100%; }
    mat-paginator { border-top: 1px solid #e0e0e0; }

    .menu-item-danger {
      color: #c62828 !important;
      mat-icon { color: #c62828 !important; }
    }
  `],
})
export class TransactionListComponent implements OnInit, OnDestroy {
  private readonly transactionService = inject(TransactionService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly destroy$ = new Subject<void>();

  loading = signal(true);
  transactions = signal<Transaction[]>([]);

  searchCtrl    = new FormControl('');
  statusCtrl    = new FormControl('');
  typeCtrl      = new FormControl('');
  startDateCtrl = new FormControl<Date | null>(this.firstDayOfMonth());
  endDateCtrl   = new FormControl<Date | null>(this.lastDayOfMonth());

  displayedColumns = ['type', 'description', 'amount', 'dueDate', 'status', 'actions'];
  pageSize  = 10;
  pageIndex = signal(0);

  activeFilters = computed(() =>
    !!this.searchCtrl.value ||
    !!this.statusCtrl.value ||
    !!this.typeCtrl.value ||
    this.startDateCtrl.value?.getTime() !== this.firstDayOfMonth().getTime() ||
    this.endDateCtrl.value?.getTime()   !== this.lastDayOfMonth().getTime()
  );

  paginatedTransactions = computed(() => {
    const start = this.pageIndex() * this.pageSize;
    return this.transactions().slice(start, start + this.pageSize);
  });

  ngOnInit(): void {
    combineLatest([
      this.searchCtrl.valueChanges.pipe(startWith(''), debounceTime(400), distinctUntilChanged()),
      this.statusCtrl.valueChanges.pipe(startWith('')),
      this.typeCtrl.valueChanges.pipe(startWith('')),
      this.startDateCtrl.valueChanges.pipe(startWith(this.startDateCtrl.value)),
      this.endDateCtrl.valueChanges.pipe(startWith(this.endDateCtrl.value)),
    ])
      .pipe(
        switchMap(([search, status, type, startDate, endDate]) => {
          this.loading.set(true);
          this.pageIndex.set(0);
          return this.transactionService.search({
            search: search ?? '',
            status: (status as TransactionStatus) ?? '',
            type: (type as TransactionType) ?? '',
            startDate: startDate ?? null,
            endDate: endDate ? this.endOfDay(endDate) : null,
          });
        }),
        takeUntil(this.destroy$),
      )
      .subscribe({
        next: (list) => {
          this.transactions.set(list);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  reloadTransactions(): void {
    this.startDateCtrl.updateValueAndValidity({ emitEvent: true, onlySelf: false });
  }

  // ---- helpers ----

  private firstDayOfMonth(): Date {
    const d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), 1);
  }

  private lastDayOfMonth(): Date {
    const d = new Date();
    return new Date(d.getFullYear(), d.getMonth() + 1, 0);
  }

  /** Garante que o filtro de data final cobre o dia inteiro */
  private endOfDay(date: Date): Date {
    const d = new Date(date);
    d.setHours(23, 59, 59, 999);
    return d;
  }

  // ---- dialogs / actions ----

  openCreateDialog(): void {
    const ref = this.dialog.open(TransactionFormComponent, { width: '560px', disableClose: true });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Transação criada com sucesso!', 'OK', { duration: 3000 });
        this.reloadTransactions();
      }
    });
  }

  openEditDialog(tx: Transaction): void {
    const data: TransactionFormData = { transaction: tx };
    const ref = this.dialog.open(TransactionFormComponent, { width: '560px', disableClose: true, data });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Transação atualizada com sucesso!', 'OK', { duration: 3000 });
        this.reloadTransactions();
      }
    });
  }

  payTransaction(tx: Transaction): void {
    const ref = this.dialog.open(PayTransactionDialogComponent, {
      width: '420px',
      disableClose: true,
      data: { description: tx.description, amount: tx.amount },
    });
    ref.afterClosed().subscribe((paymentDate: Date | null) => {
      if (!paymentDate) return;
      this.transactionService.pay(tx.id, { paymentDate: paymentDate.toISOString() }).subscribe({
        next: () => {
          this.snackBar.open('Transação marcada como paga!', 'OK', { duration: 3000 });
          this.reloadTransactions();
        },
      });
    });
  }

  cancelTransaction(tx: Transaction): void {
    this.transactionService.cancel(tx.id).subscribe({
      next: () => {
        this.snackBar.open('Transação cancelada.', 'OK', { duration: 3000 });
        this.reloadTransactions();
      },
    });
  }

  reopenTransaction(tx: Transaction): void {
    this.transactionService.reopen(tx.id).subscribe({
      next: () => {
        this.snackBar.open('Transação reaberta.', 'OK', { duration: 3000 });
        this.reloadTransactions();
      },
    });
  }

  confirmRemove(tx: Transaction): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Excluir transação',
        message: `Tem certeza que deseja excluir "<strong>${tx.description}</strong>"? Esta ação não pode ser desfeita.`,
      },
    });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) this.removeTransaction(tx);
    });
  }

  private removeTransaction(tx: Transaction): void {
    this.transactionService.remove(tx.id).subscribe({
      next: () => {
        this.snackBar.open('Transação excluída.', 'OK', { duration: 3000 });
        this.reloadTransactions();
      },
    });
  }

  clearFilters(): void {
    this.searchCtrl.setValue('');
    this.statusCtrl.setValue('');
    this.typeCtrl.setValue('');
    this.startDateCtrl.setValue(this.firstDayOfMonth());
    this.endDateCtrl.setValue(this.lastDayOfMonth());
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageIndex.set(event.pageIndex);
  }

  statusLabel(status: string): string {
    return TRANSACTION_STATUS_LABELS[status as TransactionStatus] ?? status;
  }

  typeLabel(type: string): string {
    return TRANSACTION_TYPE_LABELS[type as TransactionType] ?? type;
  }
}
