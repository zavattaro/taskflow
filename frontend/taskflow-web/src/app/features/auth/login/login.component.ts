import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { ErrorMessageComponent } from '../../../shared/components/error-message/error-message.component';
import { AppButtonComponent } from '../../../shared/components/app-button/app-button.component';
import { SuccessMessageComponent } from "../../../shared/components/success-message/success-message.component";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, ErrorMessageComponent, AppButtonComponent, SuccessMessageComponent],
  template: `
    <div class="login-page">
      <main class="login-card">
        <div class="login-brand">
          <span class="login-brand__name">TaskFlow</span>
        </div>

        <h1 class="login-title">Bem-vindo</h1>
        <p class="login-subtitle">Acesse sua conta para continuar</p>

        <form [formGroup]="form" (ngSubmit)="submit()" class="login-form">
          <div class="login-field">
            <label class="login-label" for="email">Email</label>
            <input
              class="login-input"
              id="email"
              type="email"
              formControlName="email"
              placeholder="seu@email.com"
            />
            <app-error-message
              *ngIf="form.controls.email.touched && form.controls.email.hasError('required')"
              message="Email é obrigatório.">
            </app-error-message>
            <app-error-message
              *ngIf="form.controls.email.touched && form.controls.email.hasError('email')"
              message="Email inválido.">
            </app-error-message>
          </div>

          <div class="login-field">
            <label class="login-label" for="password">Senha</label>
            <input
              class="login-input"
              id="password"
              type="password"
              formControlName="password"
              placeholder="••••••••"
            />
            <app-error-message
              *ngIf="form.controls.password.touched && form.controls.password.hasError('required')"
              message="Senha é obrigatória.">
            </app-error-message>
          </div>

          <app-button class="login-submit" type="submit" [disabled]="form.invalid || loading">
            {{ loading ? 'Entrando...' : 'Entrar' }}
          </app-button>

          <app-error-message *ngIf="errorMessage" [message]="errorMessage"></app-error-message>
          <app-success-message *ngIf="successMessage" [message]="successMessage"></app-success-message>
        </form>
      </main>
    </div>
  `,
  styles: [
    `
      .login-page {
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        background-color: var(--mat-sys-surface-container-low, #f5f5f5);
        padding: 24px;
        box-sizing: border-box;
      }

      .login-card {
        width: 100%;
        max-width: 400px;
        background: #fff;
        border-radius: 12px;
        box-shadow: 0 2px 12px rgba(0,0,0,0.12);
        padding: 40px 36px;
        box-sizing: border-box;
      }

      .login-brand {
        display: flex;
        align-items: center;
        justify-content: center;
        margin-bottom: 28px;
      }

      .login-brand__name {
        font-size: 1.5rem;
        font-weight: 700;
        color: var(--mat-sys-primary, #3949ab);
        letter-spacing: 0.02em;
      }

      .login-title {
        font-size: 1.25rem;
        font-weight: 500;
        margin: 0 0 4px;
        text-align: center;
      }

      .login-subtitle {
        font-size: 0.875rem;
        opacity: 0.65;
        text-align: center;
        margin: 0 0 24px;
      }

      .login-form {
        display: flex;
        flex-direction: column;
        gap: 16px;
      }

      .login-field {
        display: flex;
        flex-direction: column;
        gap: 6px;
      }

      .login-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: var(--mat-sys-on-surface-variant, #555);
      }

      .login-input {
        padding: 10px 12px;
        border: 1px solid #bdbdbd;
        border-radius: 4px;
        font-size: 1rem;
        outline: none;
        transition: border-color 0.15s;
        box-sizing: border-box;
        width: 100%;
      }

      .login-input:focus {
        border-color: var(--mat-sys-primary, #3949ab);
        box-shadow: 0 0 0 2px rgba(57, 73, 171, 0.15);
      }

      .login-submit {
        width: 100%;
        display: block;
        margin-top: 4px;
      }

      .login-submit ::ng-deep .app-button {
        width: 100%;
        padding: 12px 24px;
        font-size: 1rem;
      }
    `,
  ],
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loading = false;
  errorMessage = '';
  successMessage = '';

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.login(this.form.getRawValue()).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/projects']);
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Email ou senha inválidos.';
      },
    });
  }
}