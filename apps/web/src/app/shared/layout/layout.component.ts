import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavBarComponent } from '../nav-bar/nav-bar.component';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, NavBarComponent],
  template: `
    <app-nav-bar />
    <main class="content">
      <router-outlet />
    </main>
  `,
  styles: [`
    .content {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }
  `]
})
export class LayoutComponent {}
