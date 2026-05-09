import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <h2 mat-dialog-title class="dialog-title">
      Nova Categoria
    </h2>

    <mat-dialog-content>
      <form [formGroup]="form" class="form-wrapper">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Nome da Categoria</mat-label>
          <input matInput formControlName="name" placeholder="Ex: Alimentação, Moradia..." />
          @if (form.get('name')?.hasError('required') && form.get('name')?.touched) {
            <mat-error>Nome é obrigatório</mat-error>
          }
          @if (form.get('name')?.hasError('minlength') && form.get('name')?.touched) {
            <mat-error>Mínimo 2 caracteres</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Descrição (opcional)</mat-label>
          <textarea matInput formControlName="description" rows="3" placeholder="Descreva a categoria..."></textarea>
        </mat-form-field>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancelar</button>
      <button
        mat-flat-button
        class="save-btn"
        [class.saving]="saving()"
        (click)="save()"
      >
        <span class="btn-content">
          @if (saving()) {
            <mat-spinner diameter="18" />
            <span>Salvando...</span>
          } @else {
            <span>Salvar</span>
          }
        </span>
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-title {
      font-size: 18px;
      font-weight: 600;
      color: #064e3b;
      padding-bottom: 4px;
    }
    .form-wrapper {
      display: flex;
      flex-direction: column;
      min-width: 400px;
      padding-top: 8px;
      @media (max-width: 480px) { min-width: unset; }
    }
    .full-width { width: 100%; }
    mat-dialog-actions { padding: 16px 24px; gap: 8px; }

    .save-btn {
      background-color: #064e3b;
      color: white;
      min-width: 100px;
      &.saving {
        background-color: #065f46;
        opacity: 0.85;
      }
      &:disabled {
        background-color: #6ee7b7 !important;
        color: white !important;
      }
    }
    .btn-content {
      display: flex;
      align-items: center;
      gap: 8px;
      mat-spinner { --mdc-circular-progress-active-indicator-color: white; }
    }
  `],
})
export class CategoryFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly dialogRef = inject(MatDialogRef<CategoryFormComponent>);

  saving = signal(false);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: [''],
  });

  save(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const val = this.form.value;

    this.categoryService.create({
      name: val.name!,
      description: val.description || undefined,
    }).subscribe({
      next: (cat) => {
        this.saving.set(false);
        this.dialogRef.close(cat);
      },
      error: () => this.saving.set(false),
    });
  }
}
