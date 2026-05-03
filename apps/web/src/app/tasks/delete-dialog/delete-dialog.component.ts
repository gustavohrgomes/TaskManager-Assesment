import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface DeleteDialogData {
  title: string;
}

@Component({
  selector: 'app-delete-dialog',
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Delete "{{ data.title }}"?</h2>
    <mat-dialog-content>This cannot be undone.</mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="warn" [mat-dialog-close]="true">Delete</button>
    </mat-dialog-actions>
  `,
})
export class DeleteDialogComponent {
  readonly dialogRef = inject<MatDialogRef<DeleteDialogComponent>>(MatDialogRef);
  readonly data = inject<DeleteDialogData>(MAT_DIALOG_DATA);
}
