import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { LoginRequest } from '../../features/auth/models/login-request.model';
import { LoginResponse } from '../../features/auth/models/login-response.model';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = `${environment.apiBaseUrl}/Users`;

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap((response) => {
          localStorage.setItem('auth_token', response.token);
        }
      ),
    );
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  getUserDisplayName(): string {
    const token = this.getToken();

    if (!token) {
      return 'Usuário';
    }

    try {
      const payload = this.parseJwtPayload(token);
      
      const name =
        typeof payload['unique_name'] === 'string'
          ? payload['unique_name']
          : typeof payload['name'] === 'string'
          ? payload['name']
          : typeof payload['email'] === 'string'
          ? payload['email']
          : undefined;
          
      return name ?? 'Usuário';
    } catch {
      return 'Usuário';
    }
  }

  private parseJwtPayload(token: string): Record<string, unknown> {
    const parts = token.split('.');

    if (parts.length !== 3) {
      throw new Error('Token inválido.');
    }

    const base64Url = parts[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const paddedBase64 = base64.padEnd(
      base64.length + ((4 - (base64.length % 4)) % 4),
      '='
    );

    const json = atob(paddedBase64);
    return JSON.parse(json) as Record<string, unknown>;
  }
}