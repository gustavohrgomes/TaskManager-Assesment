import { Component, Inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TaskResponse, TaskStatus } from '../../core/models/task.model';

export interface TaskDialogData {
  task?: TaskResponse;
}

interface StatusOption {
  value: TaskStatus;
  label: string;
}

@Component({
  selector: 'app-task-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule, MatProgressSpinnerModule],
  templateUrl: './task-dialog.component.html',
  styleUrl: './task-dialog.component.scss'
})
export class TaskDialogComponent {
  form: FormGroup;
  isEdit: boolean;
  submitting = false;
  statusOptions: StatusOption[] = [];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<TaskDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TaskDialogData
  ) {
    this.isEdit = !!data.task;
    this.statusOptions = this.getValidStatuses(data.task?.status);

    this.form = this.fb.group({
      title: [data.task?.title || '', [Validators.required, Validators.maxLength(200)]],
      description: [data.task?.description || ''],
      dueDate: [data.task?.dueDate ? new Date(data.task.dueDate) : null],
      status: [data.task?.status || 'pending']
    });
  }

  get title(): string {
    return this.isEdit ? 'Edit Task' : 'New Task';
  }

  get submitLabel(): string {
    return this.isEdit ? 'Save' : 'Create';
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting = true;
    const value = this.form.value;
    const result = {
      title: value.title,
      description: value.description || undefined,
      dueDate: value.dueDate ? new Date(value.dueDate).toISOString() : undefined,
      status: value.status
    };
    this.dialogRef.close(result);
  }

  private getValidStatuses(current?: TaskStatus): StatusOption[] {
    if (!current) return [];
    switch (current) {
      case 'pending': return [
        { value: 'pending', label: 'Pending' },
        { value: 'inProgress', label: 'In Progress' }
      ];
      case 'inProgress': return [
        { value: 'inProgress', label: 'In Progress' },
        { value: 'completed', label: 'Completed' }
      ];
      case 'completed': return [
        { value: 'completed', label: 'Completed' }
      ];
    }
  }
}
