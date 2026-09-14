import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/authService';

// /login has no token to refresh yet, and /refresh failing means the refresh token
// itself is dead - retrying either through this same flow would just recurse forever.
function isAuthEndpoint(url: string): boolean {
  return url.includes('/api/Authentication/login') || url.includes('/api/Authentication/refresh');
}

function withAuthHeader(req: HttpRequest<unknown>, authService: AuthService): HttpRequest<unknown> {
  const token = authService.getAccessToken();
  return token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  // withCredentials is only load-bearing for /login and /refresh (they exchange the
  // HttpOnly refresh-token cookie) - harmless to set on every request since no cookie
  // exists for any other path anyway.
  const authReq = withAuthHeader(req.clone({ withCredentials: true }), authService);

  return next(authReq).pipe(
    catchError((error: unknown) => {
      const isUnauthorized = error instanceof HttpErrorResponse && error.status === 401;

      if (!isUnauthorized || isAuthEndpoint(req.url)) {
        return throwError(() => error);
      }

      return authService.refreshAccessToken().pipe(
        switchMap((refreshed) => {
          if (!refreshed) {
            authService.clearSession();
            router.navigate(['/signin']);
            return throwError(() => error);
          }
          return next(withAuthHeader(req.clone({ withCredentials: true }), authService));
        })
      );
    })
  );
};
