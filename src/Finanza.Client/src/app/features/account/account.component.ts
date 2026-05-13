import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { AccountService, ExportDataResponse } from '../../core/services/account.service';
import { AuthService } from '../../core/services/auth.service';
import { DeleteAccountDialogComponent } from './delete-account-dialog/delete-account-dialog.component';
import { DataNotExportedDialogComponent } from './data-not-exported-dialog/data-not-exported-dialog.component';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
  ],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss',
})
export class AccountComponent {
  private readonly accountService = inject(AccountService);
  private readonly auth           = inject(AuthService);
  private readonly snackBar       = inject(MatSnackBar);
  private readonly dialog         = inject(MatDialog);

  readonly user   = this.auth.user;
  exporting       = signal(false);
  hasExported     = signal(false);

  exportData(): void {
    this.exporting.set(true);
    this.accountService.exportData().subscribe({
      next: (data: ExportDataResponse) => {
        this.exporting.set(false);
        this.hasExported.set(true);
        this.downloadJson(data);
        this.snackBar.open('Dados exportados com sucesso!', 'OK', { duration: 3000 });
      },
      error: () => {
        this.exporting.set(false);
        this.snackBar.open('Erro ao exportar dados.', 'Fechar', { duration: 4000 });
      },
    });
  }

  openDeleteFlow(): void {
    if (!this.hasExported()) {
      const warn = this.dialog.open(DataNotExportedDialogComponent, {
        width: '520px',
        disableClose: true,
      });
      warn.afterClosed().subscribe(confirmed => {
        if (confirmed) this.openDeleteDialog();
      });
    } else {
      this.openDeleteDialog();
    }
  }

  private openDeleteDialog(): void {
    this.dialog.open(DeleteAccountDialogComponent, {
      width: '480px',
      disableClose: true,
    });
  }

  private downloadJson(data: ExportDataResponse): void {
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = `finanza-dados-${new Date().toISOString().slice(0, 10)}.json`;
    a.click();
    URL.revokeObjectURL(url);
  }
}
