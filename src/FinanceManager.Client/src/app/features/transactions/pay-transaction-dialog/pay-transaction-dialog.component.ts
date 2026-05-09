import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';

export interface PayTransactionDialogData {
  description: string;
  amount: number;
}

@Component({
  selector: 'app-pay-transaction-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
  ],
  template: `
    <div class="pay-dialog">
      <div class="pay-header">
        <div class="success-icon">
          <mat-icon>payments</mat-icon>
        </div>
        <h2 mat-dialog-title>Confirmar Pagamento</h2>
      </div>

      <mat-dialog-content>
        <p class="transaction-desc">
          <strong>{{ data.description }}</strong>
        </p>
        <p class="transaction-amount">
          {{ data.amount | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
        </p>

        <mat-form-field appearance="outline" class="date-field">
          <mat-label>Data de Pagamento</mat-label>
          <input
            matInput
            [matDatepicker]="picker"
            [formControl]="paymentDateCtrl"
            placeholder="DD/MM/AAAA"
          />
          <mat-datepicker-toggle matIconSuffix [for]="picker" />
          <mat-datepicker #picker />
          @if (paymentDateCtrl.hasError('required')) {
            <mat-error>A data de pagamento é obrigatória</mat-error>
          }
        </mat-form-field>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button [mat-dialog-close]="null">Cancelar</button>
        <button
          mat-flat-button
          class="confirm-btn"
          [disabled]="paymentDateCtrl.invalid"
          (click)="confirm()"
        >
          <mat-icon>check_circle</mat-icon>
          Confirmar Pagamento
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .pay-dialog { padding: 8px 0; }

    .pay-header {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 0 24px 4px;

      .success-icon {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        background: #e8f5e9;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        mat-icon { color: #2e7d32; font-size: 22px; width: 22px; height: 22px; }
      }

      h2 {
        font-size: 17px;
        font-weight: 600;
        color: #1a1a1a;
        margin: 0;
        padding: 0;
      }
    }

    mat-dialog-content {
      padding: 12px 24px 4px !important;
      display: flex;
      flex-direction: column;
      gap: 4px;

      .transaction-desc {
        font-size: 14px;
        color: #333;
        margin: 0 0 2px;
      }

      .transaction-amount {
        font-size: 22px;
        font-weight: 600;
        color: #2e7d32;
        margin: 0 0 16px;
      }

      .date-field {
        width: 100%;
      }
    }

    mat-dialog-actions {
      padding: 12px 24px 20px !important;
      gap: 8px;
    }

    .confirm-btn {
      background-color: #065f46;
      color: white;
      display: flex;
      align-items: center;
      gap: 6px;
      mat-icon { font-size: 18px; width: 18px; height: 18px; }
      &:hover { background-color: #064e3b; }
      &:disabled {
        background-color: #e0e0e0 !important;
        color: #9e9e9e !important;
      }
    }
  `],
})
export class PayTransactionDialogComponent {
  readonly data = inject<PayTransactionDialogData>(MAT_DIALOG_DATA);
  readonly dialogRef = inject(MatDialogRef<PayTransactionDialogComponent>);

  paymentDateCtrl = new FormControl<Date>(new Date(), {
    nonNullable: true,
    validators: [Validators.required],
  });

  confirm(): void {
    if (this.paymentDateCtrl.invalid) return;
    this.dialogRef.close(this.paymentDateCtrl.value);
  }
}
