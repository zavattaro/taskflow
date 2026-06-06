import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [NgIf],
  template: `
    <div *ngIf="visible" class="app-loading">
      {{ text }}
    </div>
  `,
  styles: [
    `
      .app-loading {
        margin-top: 16px;
        padding: 12px;
        text-align: center;
        opacity: 0.75;
        font-style: italic;
        border-radius: 6px;
      }
    `,
  ],
})
export class AppLoadingComponent {
  @Input() visible = false;
  @Input() text = 'Carregando...';
}