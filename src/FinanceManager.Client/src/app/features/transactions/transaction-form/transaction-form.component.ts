import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
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
import { switchMap, of, Subject } from 'rxjs';

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

        <mat-form-field appearance="outline">
          <mat-label>Data de Vencimento</mat-label>
          <input matInput [matDatepicker]="duePicker" formControlName="dueDate" />
          <mat-datepicker-toggle matIconSuffix [for]="duePicker" />
          <mat-datepicker #duePicker />
          @if (form.get('dueDate')?.hasError('required')) {
            <mat-error>Data de vencimento é obrigatória</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Data de Pagamento</mat-label>
          <input matInput [matDatepicker]="payPicker" formControlName="paymentDate"
                 placeholder="Deixe vazio se não pago" />
          <mat-datepicker-toggle matIconSuffix [for]="payPicker" />
          <mat-datepicker #payPicker />
          <mat-hint>Preencha para marcar como pago</mat-hint>
        </mat-form-field>

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
      color: #004d40;
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

    mat-dialog-content { overflow: visible; }
    mat-dialog-actions { padding: 16px 24px; gap: 8px; }

    .save-btn {
      background-color: #004d40;
      color: white;
      min-width: 100px;
      &.saving { background-color: #00695c; opacity: 0.85; }
      &:disabled {
        background-color: #80cbc4 !important;
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
export class TransactionFormComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly transactionService = inject(TransactionService);
  private readonly dialogRef = inject(MatDialogRef<TransactionFormComponent>);
  private readonly dialogData = inject<TransactionFormData>(MAT_DIALOG_DATA, { optional: true });
  private readonly destroy$ = new Subject<void>();

  categories = signal<Category[]>([]);
  saving = signal(false);

  get isEdit(): boolean {
    return !!this.dialogData?.transaction;
  }

  form = this.fb.group({
    description:     ['', [Validators.required, Validators.minLength(2)]],
    amount:          [null as number | null, [Validators.required, Validators.min(0.01)]],
    transactionType: ['', Validators.required],
    categoryId:      ['', Validators.required],
    dueDate:         [null as Date | null, Validators.required],
    paymentDate:     [null as Date | null],
  });

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(cats => {
      this.categories.set(cats);
      if (this.isEdit) this.fillForm(this.dialogData!.transaction!);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private fillForm(tx: Transaction): void {
    this.form.patchValue({
      description:     tx.description,
      amount:          tx.amount,
      transactionType: tx.type,
      categoryId:      this.categories().find(c => c.name === tx.categoryName)?.id ?? '',
      dueDate:         new Date(tx.dueDate),
      paymentDate:     tx.paymentDate ? new Date(tx.paymentDate) : null,
    });
  }

  save(): void {
    if (this.form.invalid) return;

    this.saving.set(true);
    const val         = this.form.value;
    const dueDate     = val.dueDate as Date;
    const paymentDate = val.paymentDate as Date | null;

    const save$ = this.isEdit
      ? this.transactionService.update(this.dialogData!.transaction!.id, {
          description:     val.description!,
          amount:          val.amount!,
          transactionType: val.transactionType as 'Revenue' | 'Expense',
          categoryId:      val.categoryId!,
          dueDate:         dueDate.toISOString(),
        })
      : this.transactionService.create({
          description:     val.description!,
          amount:          val.amount!,
          transactionType: val.transactionType as 'Revenue' | 'Expense',
          categoryId:      val.categoryId!,
          dueDate:         dueDate.toISOString(),
          createdAt:       new Date().toISOString(),
        });

    save$.pipe(
      switchMap(tx => {
        const alreadyPaid = this.isEdit && this.dialogData!.transaction!.status === 'Paid';
        // Chama pay() apenas se a data de pagamento foi informada e a transação ainda não está paga
        if (paymentDate && !alreadyPaid) {
          return this.transactionService.pay(tx.id, { paymentDate: paymentDate.toISOString() });
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
