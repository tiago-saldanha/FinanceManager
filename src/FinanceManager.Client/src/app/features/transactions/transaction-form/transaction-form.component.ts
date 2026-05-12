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
import { CurrencyMaskDirective } from '../../../core/directives/currency-mask.directive';

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
    CurrencyMaskDirective,
  ],
  templateUrl: './transaction-form.component.html',
  styleUrl: './transaction-form.component.scss',
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
