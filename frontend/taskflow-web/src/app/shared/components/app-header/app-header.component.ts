import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink],
  template: `
    <header
      style="
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 12px;
        padding: 16px 24px;
        border-bottom: 1px solid #ddd;
        font-family: Arial, sans-serif;
      "
    >
      <div style="display: flex; align-items: center; gap: 16px;">
        <a routerLink="/projects" style="text-decoration: none; color: #0d6efd; font-weight: bold;">
          TaskFlow
        </a>

        <span>Olá, {{ userDisplayName }}</span>
      </div>

      <button type="button" (click)="logout()" style="padding: 8px 12px;">
        Sair
      </button>
    </header>
  `,
})
export class AppHeaderComponent implements OnInit {
  private readonly authService = inject(AuthService);

  userDisplayName = 'Usuário';

  ngOnInit(): void {
    this.userDisplayName = this.authService.getUserDisplayName();
  }

  logout(): void {
    this.authService.logout();
  }
}