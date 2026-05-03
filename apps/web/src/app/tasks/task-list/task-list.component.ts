import { Component, OnInit } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { TaskService } from '../../core/services/task.service';
import { TaskResponse, TaskStatus, TaskListParams } from '../../core/models/task.model';
import { TaskCardComponent } from '../task-card/task-card.component';
import { SkeletonCardComponent } from '../skeleton-card/skeleton-card.component';
import { TaskDialogComponent, TaskDialogData } from '../task-dialog/task-dialog.component';
import { DeleteDialogComponent, DeleteDialogData } from '../delete-dialog/delete-dialog.component';

@Component({
  selector: 'app-task-list',
  imports: [MatSelectModule, MatButtonModule, MatIconModule, MatPaginatorModule, MatDialogModule, MatSnackBarModule, MatFormFieldModule, TaskCardComponent, SkeletonCardComponent],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss'
})
export class TaskListComponent implements OnInit {
  tasks: TaskResponse[] = [];
  totalCount = 0;
  loading = true;
  page = 1;
  pageSize = 10;
  statusFilter: TaskStatus | '' = '';
  sortField: 'createdAt' | 'dueDate' | 'title' | 'status' = 'createdAt';
  sortOrder: 'asc' | 'desc' = 'desc';

  constructor(
    private taskService: TaskService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.loading = true;
    const params: TaskListParams = {
      page: this.page,
      pageSize: this.pageSize,
      sort: this.sortField,
      order: this.sortOrder
    };
    if (this.statusFilter) {
      params.status = this.statusFilter;
    }
    this.taskService.list(params).subscribe({
      next: (response) => {
        this.tasks = response.items;
        this.totalCount = response.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onStatusFilterChange(value: string): void {
    this.statusFilter = value as TaskStatus | '';
    this.page = 1;
    this.loadTasks();
  }

  onSortChange(value: 'createdAt' | 'dueDate' | 'title' | 'status'): void {
    this.sortField = value;
    this.page = 1;
    this.loadTasks();
  }

  toggleSortOrder(): void {
    this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    this.loadTasks();
  }

  onPageChange(event: PageEvent): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadTasks();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '480px',
      data: {} as TaskDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.taskService.create({
          title: result.title,
          description: result.description,
          dueDate: result.dueDate
        }).subscribe({
          next: () => {
            this.snackBar.open('Task created', '', { duration: 3000 });
            this.loadTasks();
          }
        });
      }
    });
  }

  openEditDialog(task: TaskResponse): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '480px',
      data: { task } as TaskDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.taskService.update(task.taskId, {
          title: result.title,
          description: result.description,
          dueDate: result.dueDate,
          status: result.status
        }).subscribe({
          next: () => {
            this.snackBar.open('Task updated', '', { duration: 3000 });
            this.loadTasks();
          }
        });
      }
    });
  }

  openDeleteDialog(task: TaskResponse): void {
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '320px',
      data: { title: task.title } as DeleteDialogData
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.taskService.delete(task.taskId).subscribe({
          next: () => {
            this.snackBar.open('Task deleted', '', { duration: 3000 });
            this.loadTasks();
          }
        });
      }
    });
  }

  onStatusChange(event: { task: TaskResponse; newStatus: TaskStatus }): void {
    this.taskService.update(event.task.taskId, {
      title: event.task.title,
      description: event.task.description || undefined,
      dueDate: event.task.dueDate || undefined,
      status: event.newStatus
    }).subscribe({
      next: () => {
        this.snackBar.open('Task updated', '', { duration: 3000 });
        this.loadTasks();
      }
    });
  }

  get isEmpty(): boolean {
    return !this.loading && this.tasks.length === 0;
  }

  get isFiltered(): boolean {
    return this.statusFilter !== '';
  }
}
