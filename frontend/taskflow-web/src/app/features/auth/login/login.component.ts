import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  template: `
    <main style="padding: 24px; font-family: Arial, sans-serif; max-width: 420px;">
      <h1>Login</h1>

      <form
        [formGroup]="form"
        (ngSubmit)="submit()"
        style="display: flex; flex-direction: column; gap: 12px;"
      >
        <div>
          <label for="email">Email</label>
          <input
            id="email"
            type="email"
            formControlName="email"
            style="width: 100%; padding: 8px; box-sizing: border-box;"
          />

          <small
            *ngIf="form.controls.email.touched && form.controls.email.hasError('required')"
            style="color: red;"
          >
            Email é obrigatório.
          </small>

          <small
            *ngIf="form.controls.email.touched && form.controls.email.hasError('email')"
            style="color: red;"
          >
            Email inválido.
          </small>
        </div>

        <div>
          <label for="password">Senha</label>
          <input
            id="password"
            type="password"
            formControlName="password"
            style="width: 100%; padding: 8px; box-sizing: border-box;"
          />

          <small
            *ngIf="form.controls.password.touched && form.controls.password.hasError('required')"
            style="color: red;"
          >
            Senha é obrigatória.
          </small>
        </div>

        <button
          type="submit"
          [disabled]="form.invalid || loading"
          style="padding: 10px;"
        >
          {{ loading ? 'Entrando...' : 'Entrar' }}
        </button>

        <p *ngIf="errorMessage" style="color: red; margin: 0;">
          {{ errorMessage }}
        </p>

        <p *ngIf="successMessage" style="color: green; margin: 0;">
          {{ successMessage }}
        </p>
      </form>
    </main>
  `,
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