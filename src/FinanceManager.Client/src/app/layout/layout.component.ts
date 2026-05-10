import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { AuthService } from '../core/services/auth.service';

interface NavItem {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav
        #sidenav
        [mode]="isMobile() ? 'over' : 'side'"
        [opened]="!isMobile()"
        class="sidenav"
      >
        <!-- Brand -->
        <div class="brand">
          <mat-icon class="brand-icon">account_balance_wallet</mat-icon>
          <span class="brand-name">Finance Manager</span>
        </div>

        <!-- Nav links -->
        <mat-nav-list>
          @for (item of navItems; track item.path) {
            <a
              mat-list-item
              [routerLink]="item.path"
              routerLinkActive="active-link"
              [routerLinkActiveOptions]="{ exact: false }"
              (click)="isMobile() && sidenav.close()"
            >
              <mat-icon matListItemIcon>{{ item.icon }}</mat-icon>
              <span matListItemTitle>{{ item.label }}</span>
            </a>
          }
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <!-- Toolbar -->
        <mat-toolbar color="primary" class="toolbar">
          <button mat-icon-button (click)="sidenav.toggle()" class="menu-btn">
            <mat-icon>menu</mat-icon>
          </button>
          <span class="toolbar-title">Finance Manager</span>
          <span class="spacer"></span>
          <span class="toolbar-date">{{ today | date:'fullDate':'':'pt-BR' }}</span>
          <span class="user-name">{{ auth.user()?.fullName }}</span>
          <button mat-icon-button matTooltip="Sair" (click)="auth.logout()">
            <mat-icon>logout</mat-icon>
          </button>
        </mat-toolbar>

        <!-- Page content -->
        <main class="main-content">
          <router-outlet />
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .sidenav-container {
      height: 100vh;
    }

    .sidenav {
      width: 240px;
      background: #00695c;
      color: white;

      .brand {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 20px 16px;
        border-bottom: 1px solid rgba(255,255,255,0.1);
        .brand-icon {
          font-size: 32px;
          width: 32px;
          height: 32px;
          color: #80cbc4;
        }
        .brand-name {
          font-size: 17px;
          font-weight: 500;
          color: white;
        }
      }

      mat-nav-list {
        padding-top: 8px;
        a {
          color: white;
          border-radius: 0 24px 24px 0;
          margin-right: 12px;
          margin-bottom: 2px;
          transition: all 0.2s;

          &:hover {
            color: white;
            background: rgba(255,255,255,0.12) !important;
          }

          &.active-link {
            color: white;
            background: rgba(128,203,196,0.2) !important;
            border-left: 3px solid #80cbc4;
            mat-icon { color: #80cbc4; }
          }

          mat-icon { color: inherit; }
          --mdc-list-list-item-label-text-color: white;
          --mdc-list-list-item-hover-label-text-color: white;
          --mdc-list-list-item-focus-label-text-color: white;
        }
      }
    }

    .toolbar {
      position: sticky;
      top: 0;
      z-index: 100;
      background: #00695c !important;
      color: white !important;
      box-shadow: 0 2px 4px rgba(0,0,0,0.15);

      .menu-btn { margin-right: 8px; }
      .toolbar-title { font-size: 18px; font-weight: 500; }
      .spacer { flex: 1; }
      .toolbar-date {
        font-size: 13px;
        opacity: 0.85;
        text-transform: capitalize;
      }
      .user-name {
        font-size: 13px;
        opacity: 0.9;
        margin-left: 16px;
        margin-right: 4px;
        font-weight: 500;
      }
    }

    .main-content {
      min-height: calc(100vh - 64px);
      background: #e0f2f1;
    }
  `],
})
export class LayoutComponent {
  readonly auth = inject(AuthService);

  today = new Date();

  isMobile = signal(window.innerWidth < 768);

  navItems: NavItem[] = [
    { path: '/dashboard', label: 'Dashboard', icon: 'dashboard' },
    { path: '/transactions', label: 'Transações', icon: 'receipt_long' },
    { path: '/categories', label: 'Categorias', icon: 'category' },
  ];
}
