import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [NgIf],
  template: `
    <p *ngIf="visible" class="app-empty-state">
      {{ message }}
    </p>
  `,
  styles: [
    `
      .app-empty-state {
        opacity: 0.7;
        font-style: italic;
      }
    `,
  ],
})
export class AppEmptyStateComponent {
  @Input() visible = false;
  @Input() message = '';
}