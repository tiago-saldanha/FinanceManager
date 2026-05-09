import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { startWith } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';
import { CategoryFormComponent } from '../category-form/category-form.component';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTooltipModule,
  ],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1 class="page-title">Categorias</h1>
        <button mat-raised-button color="primary" (click)="openCreateDialog()">
          <mat-icon>add</mat-icon>
          Nova Categoria
        </button>
      </div>

      <!-- Search -->
      <div class="search-bar">
        <mat-form-field appearance="outline" subscriptSizing="dynamic" style="width: 320px">
          <mat-label>Buscar categoria</mat-label>
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [formControl]="searchCtrl" placeholder="Nome da categoria..." />
        </mat-form-field>
        <span class="filter-count">{{ filteredCategories().length }} categoria(s)</span>
      </div>

      @if (loading()) {
        <div class="loading-center">
          <mat-spinner diameter="48" />
        </div>
      } @else if (filteredCategories().length === 0) {
        <div class="empty-state">
          <mat-icon>category</mat-icon>
          <p>Nenhuma categoria encontrada</p>
          <button mat-raised-button color="primary" (click)="openCreateDialog()">
            Criar primeira categoria
          </button>
        </div>
      } @else {
        <!-- Summary row -->
        <div class="totals-banner">
          <div class="total-item">
            <mat-icon style="color: #2e7d32;">trending_up</mat-icon>
            <div>
              <div class="total-label">Total Recebido</div>
              <div class="total-value text-revenue">
                {{ grandTotalReceived() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
              </div>
            </div>
          </div>
          <mat-divider vertical />
          <div class="total-item">
            <mat-icon style="color: #c62828;">trending_down</mat-icon>
            <div>
              <div class="total-label">Total Gasto</div>
              <div class="total-value text-expense">
                {{ grandTotalSpent() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
              </div>
            </div>
          </div>
          <mat-divider vertical />
          <div class="total-item">
            <mat-icon style="color: #1565c0;">account_balance</mat-icon>
            <div>
              <div class="total-label">Saldo Geral</div>
              <div class="total-value"
                [class.text-revenue]="grandBalance() >= 0"
                [class.text-expense]="grandBalance() < 0">
                {{ grandBalance() | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
              </div>
            </div>
          </div>
        </div>

        <!-- Cards grid -->
        <div class="category-grid">
          @for (cat of filteredCategories(); track cat.id) {
            <mat-card class="category-card">
              <mat-card-header>
                <div class="cat-avatar" mat-card-avatar>
                  <mat-icon>label</mat-icon>
                </div>
                <mat-card-title>{{ cat.name }}</mat-card-title>
                @if (cat.description) {
                  <mat-card-subtitle>{{ cat.description }}</mat-card-subtitle>
                }
              </mat-card-header>

              <mat-card-content>
                <div class="cat-totals">
                  <div class="cat-total-item">
                    <div class="ct-label">Recebido</div>
                    <div class="ct-value text-revenue">
                      {{ cat.total.received | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                    </div>
                  </div>
                  <div class="cat-divider"></div>
                  <div class="cat-total-item">
                    <div class="ct-label">Gasto</div>
                    <div class="ct-value text-expense">
                      {{ cat.total.spent | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                    </div>
                  </div>
                  <div class="cat-divider"></div>
                  <div class="cat-total-item">
                    <div class="ct-label">Saldo</div>
                    <div class="ct-value"
                      [class.text-revenue]="cat.total.balance >= 0"
                      [class.text-expense]="cat.total.balance < 0">
                      {{ cat.total.balance | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}
                    </div>
                  </div>
                </div>

                <!-- Balance progress bar -->
                @if (cat.total.received > 0 || cat.total.spent > 0) {
                  <div class="balance-bar-wrapper" matTooltip="Percentual gasto do total recebido">
                    <div class="balance-bar">
                      <div
                        class="balance-bar-fill"
                        [style.width.%]="spentPercent(cat)"
                        [style.background]="spentPercent(cat) > 90 ? '#f44336' : spentPercent(cat) > 60 ? '#ff9800' : '#4caf50'"
                      ></div>
                    </div>
                    <span class="balance-bar-label">{{ spentPercent(cat) | number:'1.0-0' }}% gasto</span>
                  </div>
                }
              </mat-card-content>
            </mat-card>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 24px;
      .page-title { margin-bottom: 0; }
    }

    .search-bar {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 20px;
      .filter-count { color: #757575; font-size: 13px; }
    }

    .loading-center {
      display: flex;
      justify-content: center;
      padding: 48px;
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 64px;
      color: #9e9e9e;
      mat-icon { font-size: 56px; width: 56px; height: 56px; }
      p { font-size: 16px; }
    }

    .totals-banner {
      display: flex;
      align-items: center;
      gap: 24px;
      background: white;
      border-radius: 8px;
      padding: 16px 24px;
      margin-bottom: 24px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.12);

      .total-item {
        display: flex;
        align-items: center;
        gap: 12px;
        flex: 1;
        mat-icon { font-size: 28px; width: 28px; height: 28px; }
        .total-label { font-size: 12px; color: #757575; margin-bottom: 2px; }
        .total-value { font-size: 18px; font-weight: 600; }
      }

      mat-divider { height: 40px; }
    }

    .category-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 16px;
    }

    .category-card {
      transition: box-shadow 0.2s;
      &:hover { box-shadow: 0 4px 12px rgba(0,0,0,0.15); }

      .cat-avatar {
        background: #d1fae5;
        border-radius: 50%;
        width: 40px;
        height: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
        mat-icon { color: #065f46; }
      }

      .cat-totals {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-top: 16px;
        margin-bottom: 12px;

        .cat-total-item {
          flex: 1;
          text-align: center;
          .ct-label { font-size: 11px; color: #757575; margin-bottom: 4px; text-transform: uppercase; letter-spacing: 0.5px; }
          .ct-value { font-size: 14px; font-weight: 600; }
        }

        .cat-divider {
          width: 1px;
          height: 36px;
          background: #e0e0e0;
        }
      }

      .balance-bar-wrapper {
        display: flex;
        align-items: center;
        gap: 8px;
        margin-top: 4px;

        .balance-bar {
          flex: 1;
          height: 6px;
          background: #e0e0e0;
          border-radius: 3px;
          overflow: hidden;
          .balance-bar-fill {
            height: 100%;
            border-radius: 3px;
            transition: width 0.4s ease;
            max-width: 100%;
          }
        }

        .balance-bar-label {
          font-size: 11px;
          color: #757575;
          white-space: nowrap;
        }
      }
    }

    @media (max-width: 600px) {
      .totals-banner {
        flex-direction: column;
        gap: 12px;
        mat-divider { display: none; }
        .total-item { width: 100%; }
      }
    }
  `],
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  loading = signal(true);
  categories = signal<Category[]>([]);
  searchCtrl = new FormControl('');

  private searchValue = toSignal(
    this.searchCtrl.valueChanges.pipe(startWith('')),
    { initialValue: '' }
  );

  filteredCategories = computed(() => {
    const search = (this.searchValue() ?? '').toLowerCase();
    return this.categories().filter(c =>
      !search || c.name.toLowerCase().includes(search)
    );
  });

  grandTotalReceived = computed(() =>
    this.filteredCategories().reduce((s, c) => s + c.total.received, 0)
  );

  grandTotalSpent = computed(() =>
    this.filteredCategories().reduce((s, c) => s + c.total.spent, 0)
  );

  grandBalance = computed(() => this.grandTotalReceived() - this.grandTotalSpent());

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.categoryService.getAll().subscribe({
      next: (list) => {
        this.categories.set(list);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(CategoryFormComponent, {
      width: '480px',
      disableClose: true,
    });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Categoria criada com sucesso!', 'OK', { duration: 3000 });
        this.loadCategories();
      }
    });
  }

  spentPercent(cat: Category): number {
    if (cat.total.received === 0) return cat.total.spent > 0 ? 100 : 0;
    return Math.min((cat.total.spent / cat.total.received) * 100, 100);
  }
}
