import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, map, Observable, of, shareReplay, tap, throwError } from 'rxjs';
import { AuthApiResponse, AuthPayload, AuthUser } from '../types/auth.type';

const SESSION_KEY = 'auth_user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  httpClient = inject(HttpClient);
  baseUrl = `https://localhost:7254/api/auth`;

  // Deliberately not persisted (localStorage/sessionStorage) - kept only in memory so it
  // never lingers where an XSS payload could read it, and is lost on every page reload
  // by design. The app initializer re-derives it via refreshToken() on boot, using the
  // HttpOnly refresh-token cookie.
  private accessTokenSignal = signal<string | null>(null);
  private refreshInFlight$: Observable<boolean> | null = null;

  loginUser(authPayload: AuthPayload ): Observable<AuthApiResponse>{
    return this.httpClient.post<AuthApiResponse>(this.baseUrl+'/login',authPayload)
            .pipe(
              catchError(this.handleError)
            );
  }

  logoutUser(): Observable<{ success: boolean; message: string }>{
    return this.httpClient.post<{ success: boolean; message: string }>(this.baseUrl+'/logout', {})
            .pipe(
              catchError(this.handleError)
            );
  }

  // Uses the HttpOnly refresh-token cookie (sent automatically by the browser) to obtain
  // a fresh access token without the user re-entering credentials. Coalesces concurrent
  // callers (app initializer, a guard, several requests 401-ing at once) into a single
  // in-flight HTTP call - the interceptor's own queueing sits on top of this.
  refreshToken(): Observable<boolean> {
    if (this.refreshInFlight$) {
      return this.refreshInFlight$;
    }

    this.refreshInFlight$ = this.httpClient.post<AuthApiResponse>(this.baseUrl + '/refresh', {}).pipe(
      tap((data) => {
        if (data.success) {
          this.setAccessToken(data.result.accessToken);
          this.setSession({
            userId: data.result.userId,
            username: data.result.username,
            roleName: data.result.roleName
          });
        }
      }),
      map((data) => data.success),
      catchError(() => of(false)),
      finalize(() => { this.refreshInFlight$ = null; }),
      shareReplay(1)
    );

    return this.refreshInFlight$;
  }

  getAccessToken(): string | null {
    return this.accessTokenSignal();
  }

  setAccessToken(token: string): void {
    this.accessTokenSignal.set(token);
  }

  clearAccessToken(): void {
    this.accessTokenSignal.set(null);
  }

  setSession(user: AuthUser): void {
    if (typeof sessionStorage !== 'undefined') {
      sessionStorage.setItem(SESSION_KEY, JSON.stringify(user));
    }
  }

  clearSession(): void {
    this.clearAccessToken();
    if (typeof sessionStorage !== 'undefined') {
      sessionStorage.removeItem(SESSION_KEY);
    }
  }

  getUser(): AuthUser | null {
    if (typeof sessionStorage === 'undefined') {
      return null;
    }
    const raw = sessionStorage.getItem(SESSION_KEY);
    return raw ? JSON.parse(raw) as AuthUser : null;
  }

  private handleError(error: HttpErrorResponse) {
    const backendMessage = error.error?.message || error.error?.errors?.[0];
    const message = backendMessage || (error.status === 0
      ? 'Unable to reach the server. Please check your connection and try again.'
      : 'Something went wrong. Please try again.');
    return throwError(() => new Error(message));
  }
}
