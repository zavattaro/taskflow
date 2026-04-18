import { inject } from '@angular/core';
import {
  HttpErrorResponse,
  HttpInterceptorFn,
} from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  const isPublicRoute =
    req.url.includes('/api/Users/login') ||
    req.url.includes('/api/Users/register');

  const token = localStorage.getItem('auth_token');

  const authReq =
    !isPublicRoute && token
      ? req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`,
          },
        })
      : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      const isUnauthorized = error.status === 401;

      if (isUnauthorized && !isPublicRoute) {
        localStorage.removeItem('auth_token');
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};