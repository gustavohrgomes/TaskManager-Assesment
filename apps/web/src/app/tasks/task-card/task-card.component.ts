import { Component, Input, Output, EventEmitter } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TaskResponse, TaskStatus } from '../../core/models/task.model';

@Component({
  selector: 'app-task-card',
  imports: [DatePipe, MatCardModule, MatChipsModule, MatButtonModule, MatIconModule],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.scss'
})
export class TaskCardComponent {
  @Input({ required: true }) task!: TaskResponse;
  @Output() edit = new EventEmitter<TaskResponse>();
  @Output() delete = new EventEmitter<TaskResponse>();
  @Output() statusChange = new EventEmitter<{ task: TaskResponse; newStatus: TaskStatus }>();

  get statusClass(): string {
    return `status-${this.task.status}`;
  }

  get statusLabel(): string {
    switch (this.task.status) {
      case 'pending': return 'Pending';
      case 'inProgress': return 'In Progress';
      case 'completed': return 'Completed';
    }
  }

  get canStart(): boolean {
    return this.task.status === 'pending';
  }

  get canComplete(): boolean {
    return this.task.status === 'inProgress';
  }

  onStart(): void {
    this.statusChange.emit({ task: this.task, newStatus: 'inProgress' });
  }

  onComplete(): void {
    this.statusChange.emit({ task: this.task, newStatus: 'completed' });
  }

  onEdit(): void {
    this.edit.emit(this.task);
  }

  onDelete(): void {
    this.delete.emit(this.task);
  }
}
