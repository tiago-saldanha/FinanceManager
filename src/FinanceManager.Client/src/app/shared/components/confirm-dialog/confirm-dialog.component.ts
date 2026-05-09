import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <div class="confirm-dialog">
      <div class="confirm-header">
        <div class="danger-icon">
          <mat-icon>warning_amber</mat-icon>
        </div>
        <h2 mat-dialog-title>{{ data.title }}</h2>
      </div>

      <mat-dialog-content>
        <p [innerHTML]="data.message"></p>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button [mat-dialog-close]="false">
          {{ data.cancelLabel ?? 'Cancelar' }}
        </button>
        <button mat-flat-button class="confirm-btn" [mat-dialog-close]="true">
          <mat-icon>delete_outline</mat-icon>
          {{ data.confirmLabel ?? 'Excluir' }}
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .confirm-dialog { padding: 8px 0; }

    .confirm-header {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 0 24px 4px;

      .danger-icon {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        background: #fef2f2;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        mat-icon { color: #c62828; font-size: 22px; width: 22px; height: 22px; }
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
      padding: 8px 24px 4px !important;
      p {
        color: #555;
        font-size: 14px;
        line-height: 1.6;
        margin: 0;
      }
    }

    mat-dialog-actions {
      padding: 12px 24px 20px !important;
      gap: 8px;
    }

    .confirm-btn {
      background-color: #c62828;
      color: white;
      display: flex;
      align-items: center;
      gap: 6px;
      mat-icon { font-size: 18px; width: 18px; height: 18px; }
      &:hover { background-color: #b71c1c; }
    }
  `],
})
export class ConfirmDialogComponent {
  readonly data = inject<ConfirmDialogData>(MAT_DIALOG_DATA);
  readonly dialogRef = inject(MatDialogRef<ConfirmDialogComponent>);
}
