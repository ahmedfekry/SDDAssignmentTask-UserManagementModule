import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, finalize, map, Observable, of, shareReplay, tap, throwError } from 'rxjs';
import { AuthApiResponse, AuthPayload, AuthUser } from '../types/auth.type';

const SESSION_KEY = 'auth_user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  httpClient = inject(HttpClient);
  baseUrl = `https://localhost:7254/api/Authentication`;


  private accessToken: string | null = null;
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

  refreshAccessToken(): Observable<boolean> {
    if (this.refreshInFlight$) {
      return this.refreshInFlight$;
    }

    this.refreshInFlight$ = this.httpClient.post<AuthApiResponse>(this.baseUrl + '/refresh', {}).pipe(
      tap((data) => {
        if (data.success) {
          this.accessToken = data.result.accessToken;
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
    return this.accessToken;
  }

  setAccessToken(token: string): void {
    this.accessToken = token;
  }

  clearAccessToken(): void {
    this.accessToken = null;
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
