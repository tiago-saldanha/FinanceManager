import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-data-not-exported-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './data-not-exported-dialog.component.html',
  styleUrl: './data-not-exported-dialog.component.scss',
})
export class DataNotExportedDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<DataNotExportedDialogComponent>);

  confirm(): void { this.dialogRef.close(true); }
}
