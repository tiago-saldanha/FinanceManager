import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';

export interface CategoryFormData {
  category?: Category;
}

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
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss',
})
export class CategoryFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly dialogRef = inject(MatDialogRef<CategoryFormComponent>);
  private readonly data = inject<CategoryFormData>(MAT_DIALOG_DATA, { optional: true });

  saving = signal(false);
  isEditMode = signal(false);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: [''],
  });

  ngOnInit(): void {
    if (this.data?.category) {
      this.isEditMode.set(true);
      this.form.patchValue({
        name: this.data.category.name,
        description: this.data.category.description ?? '',
      });
    }
  }

  save(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const val = this.form.value;
    const request = {
      name: val.name!,
      description: val.description || undefined,
    };

    const operation = this.isEditMode()
      ? this.categoryService.update(this.data!.category!.id, request)
      : this.categoryService.create(request);

    operation.subscribe({
      next: (cat) => {
        this.saving.set(false);
        this.dialogRef.close(cat);
      },
      error: () => this.saving.set(false),
    });
  }
}
