import { NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';


@Component({
  selector: 'app-success-message',
  standalone: true,
  imports: [NgIf],
  template: `
    <small *ngIf="message" class="success-text">
      {{ message }}
    </small>
  `,
  styles: [
    `
      .success-text {
        color: green;
      }
    `,
  ],
})
export class SuccessMessageComponent {
  @Input() message = '';
}