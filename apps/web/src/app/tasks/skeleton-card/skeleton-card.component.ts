import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-skeleton-card',
  imports: [MatCardModule],
  template: `
    <mat-card class="skeleton-card">
      <div class="skeleton-line title"></div>
      <div class="skeleton-line chip"></div>
      <div class="skeleton-line date"></div>
      <div class="skeleton-line desc"></div>
      <div class="skeleton-line desc short"></div>
    </mat-card>
  `,
  styleUrl: './skeleton-card.component.scss'
})
export class SkeletonCardComponent {}
