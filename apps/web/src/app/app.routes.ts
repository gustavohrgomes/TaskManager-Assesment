import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent) },
  {
    path: '',
    loadComponent: () => import('./shared/layout/layout.component').then(m => m.LayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: 'tasks', loadComponent: () => import('./tasks/task-list/task-list.component').then(m => m.TaskListComponent) },
      { path: '', redirectTo: 'tasks', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'tasks' }
];
