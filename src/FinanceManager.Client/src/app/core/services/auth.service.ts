import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '@environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

const TOKEN_KEY    = 'fm_token';
const USER_KEY     = 'fm_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http   = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly _user = signal<Pick<AuthResponse, 'email' | 'fullName'> | null>(
    this.loadUserFromStorage()
  );

  readonly user          = this._user.asReadonly();
  readonly isLoggedIn    = computed(() => this._user() !== null);

  // ── Login ────────────────────────────────────────────────────────────────
  login(request: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request).pipe(
      tap(response => this.persist(response))
    );
  }

  // ── Register ─────────────────────────────────────────────────────────────
  register(request: RegisterRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, request).pipe(
      tap(response => this.persist(response))
    );
  }

  // ── Logout ────────────────────────────────────────────────────────────────
  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._user.set(null);
    this.router.navigate(['/login']);
  }

  // ── Token ─────────────────────────────────────────────────────────────────
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  // ── Helpers ───────────────────────────────────────────────────────────────
  private persist(response: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(USER_KEY, JSON.stringify({
      email:    response.email,
      fullName: response.fullName
    }));
    this._user.set({ email: response.email, fullName: response.fullName });
  }

  private loadUserFromStorage(): Pick<AuthResponse, 'email' | 'fullName'> | null {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }
}
