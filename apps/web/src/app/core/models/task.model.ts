export type TaskStatus = 'pending' | 'inProgress' | 'completed';

export interface TaskResponse {
  taskId: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface TaskListResponse {
  items: TaskResponse[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  dueDate?: string;
  status: TaskStatus;
}

export interface TaskListParams {
  page?: number;
  pageSize?: number;
  status?: TaskStatus;
  dueBefore?: string;
  sort?: 'createdAt' | 'dueDate' | 'title' | 'status';
  order?: 'asc' | 'desc';
}
