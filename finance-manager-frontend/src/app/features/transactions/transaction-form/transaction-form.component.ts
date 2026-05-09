import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { switchMap, of } from 'rxjs';

import { CategoryService } from '../../../core/services/category.service';
import { TransactionService } from '../../../core/services/transaction.service';
import { Transaction } from '../../../core/models/transaction.model';
import { Category } from '../../../core/models/category.model';

export interface TransactionFormData {
  transaction?: Transaction;
}

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatDividerModule,
  ],
  template: `
    <h2 mat-dialog-title class="dialog-title">
      {{ isEdit ? 'Editar Transação' : 'Nova Transação' }}
    </h2>

    <mat-dialog-content>
      <form [formGroup]="form" class="form-grid">

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Descrição</mat-label>
          <input matInput formControlName="description" placeholder="Ex: Salário, Aluguel..." />
          @if (form.get('description')?.hasError('required')) {
            <mat-error>Descrição é obrigatória</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Valor (R$)</mat-label>
          <input matInput type="number" formControlName="amount" min="0.01" step="0.01" />
          @if (form.get('amount')?.hasError('required')) {
            <mat-error>Valor é obrigatório</mat-error>
          }
          @if (form.get('amount')?.hasError('min')) {
            <mat-error>Valor deve ser maior que zero</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Tipo</mat-label>
          <mat-select formControlName="transactionType">
            <mat-option value="Revenue">Receita</mat-option>
            <mat-option value="Expense">Despesa</mat-option>
          </mat-select>
          @if (form.get('transactionType')?.hasError('required')) {
            <mat-error>Tipo é obrigatório</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Categoria</mat-label>
          <mat-select formControlName="categoryId">
            @for (cat of categories(); track cat.id) {
              <mat-option [value]="cat.id">{{ cat.name }}</mat-option>
            }
          </mat-select>
          @if (form.get('categoryId')?.hasError('required')) {
            <mat-error>Categoria é obrigatória</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Data de Vencimento</mat-label>
          <input matInput [matDatepicker]="duePicker" formControlName="dueDate" />
          <mat-datepicker-toggle matIconSuffix [for]="duePicker" />
          <mat-datepicker #duePicker />
          @if (form.get('dueDate')?.hasError('required')) {
            <mat-error>Data de vencimento é obrigatória</mat-error>
          }
        </mat-form-field>

        <mat-divider class="full-width divider" />

        <!-- Checkbox quitar -->
        <div class="paid-row full-width">
          <mat-checkbox formControlName="isPaid" color="primary" class="paid-checkbox">
            Marcar como pago
          </mat-checkbox>
          @if (form.get('isPaid')?.value) {
            <span class="paid-hint">
              <mat-icon class="hint-icon">info</mat-icon>
              Data de pagamento: {{ dueDateLabel }}
            </span>
          }
        </div>

      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancelar</button>
      <button
        mat-flat-button
        class="save-btn"
        [class.saving]="saving()"
        (click)="save()"
      >
        <span class="btn-content">
          @if (saving()) {
            <mat-spinner diameter="18" />
            <span>Salvando...</span>
          } @else {
            <span>{{ isEdit ? 'Salvar alterações' : 'Salvar' }}</span>
          }
        </span>
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-title {
      font-size: 18px;
      font-weight: 600;
      color: #064e3b;
      padding-bottom: 4px;
    }

    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 0 16px;
      min-width: 480px;
      padding-top: 8px;

      @media (max-width: 600px) {
        grid-template-columns: 1fr;
        min-width: unset;
      }
    }

    .full-width { grid-column: 1 / -1; }

    .divider {
      margin: 4px 0 12px;
    }

    .paid-row {
      display: flex;
      align-items: center;
      gap: 16px;
      padding-bottom: 4px;
      flex-wrap: wrap;
    }

    .paid-checkbox {
      --mdc-checkbox-selected-icon-color: #065f46;
      --mdc-checkbox-selected-focus-icon-color: #065f46;
      --mdc-checkbox-selected-hover-icon-color: #065f46;
      --mdc-checkbox-selected-pressed-icon-color: #065f46;
      --mdc-checkbox-selected-checkmark-color: white;
    }

    .paid-hint {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 13px;
      color: #2e7d32;
      font-weight: 500;

      .hint-icon {
        font-size: 15px;
        width: 15px;
        height: 15px;
      }
    }

    mat-dialog-content { overflow: visible; }
    mat-dialog-actions { padding: 16px 24px; gap: 8px; }

    .save-btn {
      background-color: #064e3b;
      color: white;
      min-width: 100px;
      &.saving { background-color: #065f46; opacity: 0.85; }
      &:disabled {
        background-color: #6ee7b7 !important;
        color: white !important;
      }
    }

    .btn-content {
      display: flex;
      align-items: center;
      gap: 8px;
      mat-spinner { --mdc-circular-progress-active-indicator-color: white; }
    }
  `],
})
export class TransactionFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly transactionService = inject(TransactionService);
  private readonly dialogRef = inject(MatDialogRef<TransactionFormComponent>);
  private readonly dialogData = inject<TransactionFormData>(MAT_DIALOG_DATA, { optional: true });

  categories = signal<Category[]>([]);
  saving = signal(false);

  get isEdit(): boolean {
    return !!this.dialogData?.transaction;
  }

  get dueDateLabel(): string {
    const d = this.form.get('dueDate')?.value as Date | null;
    if (!d) return '—';
    return d.toLocaleDateString('pt-BR');
  }

  form = this.fb.group({
    description: ['', [Validators.required, Validators.minLength(2)]],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    transactionType: ['', Validators.required],
    categoryId: ['', Validators.required],
    dueDate: [null as Date | null, Validators.required],
    isPaid: [false],
  });

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(cats => {
      this.categories.set(cats);
      if (this.isEdit) this.fillForm(this.dialogData!.transaction!);
    });
  }

  private fillForm(tx: Transaction): void {
    this.form.patchValue({
      description: tx.description,
      amount: tx.amount,
      transactionType: tx.type,
      categoryId: this.categories().find(c => c.name === tx.categoryName)?.id ?? '',
      dueDate: new Date(tx.dueDate),
      isPaid: tx.status === 'Paid',
    });
  }

  save(): void {
    if (this.form.invalid) return;

    this.saving.set(true);
    const val = this.form.value;
    const dueDate = val.dueDate as Date;
    const shouldPay = !!val.isPaid;

    const save$ = this.isEdit
      ? this.transactionService.update(this.dialogData!.transaction!.id, {
          description: val.description!,
          amount: val.amount!,
          transactionType: val.transactionType as 'Revenue' | 'Expense',
          categoryId: val.categoryId!,
          dueDate: dueDate.toISOString(),
        })
      : this.transactionService.create({
          description: val.description!,
          amount: val.amount!,
          transactionType: val.transactionType as 'Revenue' | 'Expense',
          categoryId: val.categoryId!,
          dueDate: dueDate.toISOString(),
          createdAt: new Date().toISOString(),
        });

    save$.pipe(
      switchMap(tx => {
        const alreadyPaid = this.isEdit && this.dialogData!.transaction!.status === 'Paid';
        if (shouldPay && !alreadyPaid) {
          return this.transactionService.pay(tx.id, { paymentDate: dueDate.toISOString() });
        }
        return of(tx);
      }),
    ).subscribe({
      next: (tx) => {
        this.saving.set(false);
        this.dialogRef.close(tx);
      },
      error: () => this.saving.set(false),
    });
  }
}
