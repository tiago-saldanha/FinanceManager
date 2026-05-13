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
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
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
