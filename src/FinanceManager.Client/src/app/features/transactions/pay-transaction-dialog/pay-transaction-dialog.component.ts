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
  templateUrl: './pay-transaction-dialog.component.html',
  styleUrl: './pay-transaction-dialog.component.scss',
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
