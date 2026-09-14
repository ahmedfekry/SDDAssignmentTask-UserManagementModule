import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/authService';

// /login has no token to refresh yet, and /refresh failing means the refresh token
// itself is dead - retrying either through this same flow would just recurse forever.
function isAuthEndpoint(url: string): boolean {
  return url.includes('/api/auth/login') || url.includes('/api/auth/refresh');
}

function withAuthHeader(req: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
  return token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
}

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  // withCredentials is only load-bearing for /login, /refresh and /logout (they exchange
  // the HttpOnly refresh-token cookie) - harmless to set on every request since no cookie
  // exists for any other path anyway. The Bearer header is skipped for the auth endpoints
  // themselves; they don't need one and it'd just be a stale/absent token.
  const withCreds = req.clone({ withCredentials: true });
  const authReq = isAuthEndpoint(req.url) ? withCreds : withAuthHeader(withCreds, authService.getAccessToken());

  return next(authReq).pipe(
    catchError((error: unknown) => {
      const isUnauthorized = error instanceof HttpErrorResponse && error.status === 401;

      if (!isUnauthorized || isAuthEndpoint(req.url)) {
        return throwError(() => error);
      }

      // authService.refreshToken() coalesces concurrent callers into a single in-flight
      // HTTP call (shareReplay) - if several requests 401 at once, they all subscribe to
      // the same refresh and get released together once it resolves, which is effectively
      // the "queue requests while refreshing" behavior without needing a separate queue.
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
