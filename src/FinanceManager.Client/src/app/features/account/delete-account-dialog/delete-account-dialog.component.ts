import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AccountService } from '../../../core/services/account.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-delete-account-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './delete-account-dialog.component.html',
  styleUrl: './delete-account-dialog.component.scss',
})
export class DeleteAccountDialogComponent {
  private readonly fb             = inject(FormBuilder);
  private readonly accountService = inject(AccountService);
  private readonly auth           = inject(AuthService);
  private readonly dialogRef      = inject(MatDialogRef<DeleteAccountDialogComponent>);

  deleting     = signal(false);
  hidePassword = signal(true);

  form = this.fb.group({
    password: ['', Validators.required],
  });

  confirm(): void {
    if (this.form.invalid) return;
    this.deleting.set(true);

    this.accountService.deleteAccount(this.form.value.password!).subscribe({
      next: () => {
        this.deleting.set(false);
        this.dialogRef.close(true);
        this.auth.logout();
      },
      error: () => this.deleting.set(false),
    });
  }
}
