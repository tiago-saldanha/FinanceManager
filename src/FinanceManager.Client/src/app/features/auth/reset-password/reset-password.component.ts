import { Component, inject, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';

function passwordsMatch(control: AbstractControl): ValidationErrors | null {
  const parent = control.parent;
  if (!parent) return null;
  return parent.get('newPassword')?.value === control.value ? null : { mismatch: true };
}

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
})
export class ResetPasswordComponent implements OnInit {
  private readonly auth  = inject(AuthService);
  private readonly fb    = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly loading             = signal(false);
  readonly done                = signal(false);
  readonly invalidLink         = signal(false);
  readonly hideNewPassword     = signal(true);
  readonly hideConfirmPassword = signal(true);

  private email = '';
  private token = '';

  form = this.fb.group({
    newPassword:     ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required, passwordsMatch]],
  });

  ngOnInit(): void {
    const params = this.route.snapshot.queryParamMap;
    this.email = params.get('email') ?? '';
    this.token = params.get('token') ?? '';

    if (!this.email || !this.token) {
      this.invalidLink.set(true);
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);

    const { newPassword } = this.form.getRawValue();
    this.auth.resetPassword({ email: this.email, token: this.token, newPassword: newPassword! }).subscribe({
      next: () => {
        this.loading.set(false);
        this.done.set(true);
      },
      error: () => this.loading.set(false),
    });
  }

  onNewPasswordInput(): void {
    this.form.get('confirmPassword')?.updateValueAndValidity();
  }
}
