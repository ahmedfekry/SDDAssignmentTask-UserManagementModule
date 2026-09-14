import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/authService';

function isAuthEndpoint(url: string): boolean {
  return url.includes('/api/auth/login') || url.includes('/api/auth/refresh');
}

function withAuthHeader(req: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
  return token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
}

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  const withCreds = req.clone({ withCredentials: true });
  const authReq = isAuthEndpoint(req.url) ? withCreds : withAuthHeader(withCreds, authService.getAccessToken());

  return next(authReq).pipe(
    catchError((error: unknown) => {
      const isUnauthorized = error instanceof HttpErrorResponse && error.status === 401;

      if (!isUnauthorized || isAuthEndpoint(req.url)) {
        return throwError(() => error);
      }

      return authService.refreshToken().pipe(
        switchMap((refreshed) => {
          if (!refreshed) {
            authService.clearSession();
            router.navigate(['/signin']);
            return throwError(() => error);
          }
          return next(withAuthHeader(req.clone({ withCredentials: true }), authService.getAccessToken()));
        })
      );
    })
  );
};
