import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/authService';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  // The access token is an HttpOnly cookie - withCredentials makes the browser
  // send it (and the XSRF-TOKEN cookie) and accept the Set-Cookie on responses.
  const authReq = req.clone({ withCredentials: true });

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
