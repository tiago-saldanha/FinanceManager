import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
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
  template: `
    <div class="auth-container">
      <mat-card class="auth-card">

        <!-- Brand -->
        <div class="brand">
          <mat-icon class="brand-icon">account_balance_wallet</mat-icon>
          <span class="brand-name">Finance Manager</span>
        </div>

        <mat-card-header>
          <mat-card-title>Entrar na conta</mat-card-title>
          <mat-card-subtitle>Bem-vindo de volta!</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form">

            <mat-form-field appearance="outline">
              <mat-label>E-mail</mat-label>
              <input matInput type="email" formControlName="email" autocomplete="email" />
              <mat-icon matSuffix>email</mat-icon>
              @if (form.get('email')?.hasError('required') && form.get('email')?.touched) {
                <mat-error>E-mail é obrigatório</mat-error>
              } @else if (form.get('email')?.hasError('email') && form.get('email')?.touched) {
                <mat-error>E-mail inválido</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Senha</mat-label>
              <input matInput [type]="hidePassword() ? 'password' : 'text'"
                     formControlName="password" autocomplete="current-password" />
              <button mat-icon-button matSuffix type="button"
                      (click)="hidePassword.set(!hidePassword())">
                <mat-icon>{{ hidePassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
              @if (form.get('password')?.hasError('required') && form.get('password')?.touched) {
                <mat-error>Senha é obrigatória</mat-error>
              }
            </mat-form-field>

            <button mat-flat-button class="submit-btn" type="submit"
                    [disabled]="form.invalid || loading()">
              @if (loading()) {
                <mat-spinner diameter="20" />
              } @else {
                Entrar
              }
            </button>

          </form>
        </mat-card-content>

        <mat-card-actions>
          <p class="footer-text">
            Não tem uma conta?
            <a routerLink="/register" class="link">Cadastre-se</a>
          </p>
        </mat-card-actions>

      </mat-card>
    </div>
  `,
  styles: [`
    .auth-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #004d40 0%, #00695c 50%, #047857 100%);
      padding: 16px;
    }

    .auth-card {
      width: 100%;
      max-width: 420px;
      border-radius: 16px;
      padding: 8px 8px 16px;
      box-shadow: 0 25px 50px rgba(0, 0, 0, 0.35);
    }

    .brand {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 10px;
      padding: 20px 0 8px;

      .brand-icon {
        font-size: 36px;
        width: 36px;
        height: 36px;
        color: #00695c;
      }
      .brand-name {
        font-size: 20px;
        font-weight: 600;
        color: #004d40;
      }
    }

    mat-card-header {
      justify-content: center;
      text-align: center;
      margin-bottom: 8px;
      mat-card-title  { font-size: 22px; font-weight: 600; color: #1f2937; }
      mat-card-subtitle { margin-top: 4px; }
    }

    .form {
      display: flex;
      flex-direction: column;
      gap: 4px;
      padding-top: 8px;

      mat-form-field { width: 100%; }
    }

    .submit-btn {
      width: 100%;
      height: 48px;
      margin-top: 8px;
      font-size: 16px;
      font-weight: 500;
      background-color: #00695c;
      color: white;
      border-radius: 8px;

      mat-spinner { margin: 0 auto; }
    }

    mat-card-actions {
      padding: 8px 16px 0;
    }

    .footer-text {
      text-align: center;
      color: #6b7280;
      font-size: 14px;
      margin: 0;

      .link {
        color: #00695c;
        font-weight: 500;
        text-decoration: none;
        &:hover { text-decoration: underline; }
      }
    }
  `],
})
export class LoginComponent {
  private readonly auth   = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb     = inject(FormBuilder);

  readonly loading     = signal(false);
  readonly hidePassword = signal(true);

  form = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);

    const { email, password } = this.form.getRawValue();
    this.auth.login({ email: email!, password: password! }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => this.loading.set(false),
    });
  }
}
