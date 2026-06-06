import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  template: `
    <button
      [type]="type"
      [disabled]="disabled"
      (click)="handleClick($event)"
      class="app-button"
    >
      <ng-content></ng-content>
    </button>
  `,
  styles: [
    `
      .app-button {
        padding: 10px 24px;
        cursor: pointer;
        background-color: var(--mat-sys-primary, #3949ab);
        color: var(--mat-sys-on-primary, #fff);
        border: none;
        border-radius: 4px;
        font-size: 0.875rem;
        font-weight: 500;
        letter-spacing: 0.04em;
        transition: opacity 0.15s;
        white-space: nowrap;
      }

      .app-button:hover:not(:disabled) {
        opacity: 0.88;
      }

      .app-button:disabled {
        cursor: not-allowed;
        opacity: 0.38;
      }
    `,
  ],
})
export class AppButtonComponent {
  @Input() type: 'button' | 'submit' = 'button';
  @Input() disabled = false;

  @Output() clicked = new EventEmitter<Event>();

  handleClick(event: Event): void {
    this.clicked.emit(event);
  }
}