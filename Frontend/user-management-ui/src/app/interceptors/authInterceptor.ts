import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/authService';

const XSRF_COOKIE_NAME = 'XSRF-TOKEN';
const XSRF_HEADER_NAME = 'X-XSRF-TOKEN';
const SAFE_METHODS = new Set(['GET', 'HEAD', 'OPTIONS']);

function readCookie(name: string): string | null {
  if (typeof document === 'undefined') {
    return null;
  }
  const prefix = name + '=';
  const match = document.cookie.split('; ').find(row => row.startsWith(prefix));
  return match ? decodeURIComponent(match.slice(prefix.length)) : null;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  let authReq = req.clone({ withCredentials: true });

  if (!SAFE_METHODS.has(req.method.toUpperCase())) {
    const csrfToken = readCookie(XSRF_COOKIE_NAME);
    if (csrfToken) {
      authReq = authReq.clone({ setHeaders: { [XSRF_HEADER_NAME]: csrfToken } });
    }
  }

  return next(authReq).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        authService.clearSession();
        router.navigate(['/signin']);
      }
      return throwError(() => error);
    })
  );
};
