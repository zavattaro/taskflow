import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [NgIf],
  template: `
    <small *ngIf="message" class="error-text">
      {{ message }}
    </small>
  `,
  styles: [
    `
      .error-text {
        color: red;
      }
    `,
  ],
})
export class ErrorMessageComponent {
  @Input() message = '';
}
