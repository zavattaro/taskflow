import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink],
  template: `
    <header class="app-header">
      <div class="app-header__left">
        <a routerLink="/projects" class="app-header__brand">TaskFlow</a>
        <span class="app-header__greeting">Olá, {{ userDisplayName }}</span>
      </div>
      <button type="button" class="app-header__logout" (click)="logout()">Sair</button>
    </header>
  `,
  styles: [
    `
      .app-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 12px;
        padding: 0 24px;
        height: 56px;
        background-color: var(--mat-sys-primary, #8c00ba);
        color: var(--mat-sys-on-primary, #fff);
        box-shadow: 0 2px 4px rgba(0,0,0,0.2);
      }

      .app-header__left {
        display: flex;
        align-items: center;
        gap: 20px;
      }

      .app-header__brand {
        text-decoration: none;
        font-weight: 600;
        font-size: 1.1rem;
        color: var(--mat-sys-on-primary, #fff);
        letter-spacing: 0.02em;
      }

      .app-header__greeting {
        font-size: 0.9rem;
        opacity: 0.85;
      }

      .app-header__logout {
        padding: 6px 16px;
        cursor: pointer;
        background: transparent;
        color: var(--mat-sys-on-primary, #fff);
        border: 1px solid rgba(255,255,255,0.5);
        border-radius: 4px;
        font-size: 0.875rem;
        font-weight: 500;
        transition: background 0.15s;
      }

      .app-header__logout:hover {
        background: rgba(255,255,255,0.12);
      }
    `,
  ],
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