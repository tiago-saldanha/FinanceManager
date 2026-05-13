import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

function passwordsMatch(control: AbstractControl): ValidationErrors | null {
  const parent = control.parent;
  if (!parent) return null;
  return parent.get('newPassword')?.value === control.value ? null : { mismatch: true };
}

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss',
})
export class ChangePasswordComponent {
  private readonly auth     = inject(AuthService);
  private readonly fb       = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  readonly saving              = signal(false);
  readonly hideCurrentPassword = signal(true);
  readonly hideNewPassword     = signal(true);
  readonly hideConfirmPassword = signal(true);

  form = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword:     ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required, passwordsMatch]],
  });

  onSubmit(): void {
    if (this.form.invalid) return;
    this.saving.set(true);

    const { currentPassword, newPassword } = this.form.getRawValue();
    this.auth.changePassword({ currentPassword: currentPassword!, newPassword: newPassword! }).subscribe({
      next: () => {
        this.saving.set(false);
        this.form.reset();
        this.snackBar.open('Senha alterada com sucesso!', 'Fechar', {
          duration: 5000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['success-snackbar'],
        });
      },
      error: () => this.saving.set(false),
    });
  }

  onNewPasswordInput(): void {
    this.form.get('confirmPassword')?.updateValueAndValidity();
  }
}
