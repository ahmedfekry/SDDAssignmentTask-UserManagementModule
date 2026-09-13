import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { AuthApiResponse, AuthPayload } from '../types/auth.type';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  httpClient = inject(HttpClient);
  baseUrl = `https://localhost:7254/api/Authentication`;

  loginUser(authPayload: AuthPayload ): Observable<AuthApiResponse>{
    return this.httpClient.post<AuthApiResponse>(this.baseUrl+'/login',authPayload)
            .pipe(
              catchError(this.handleError)
            );
  }

  private handleError(error: HttpErrorResponse) {
    const backendMessage = error.error?.message || error.error?.errors?.[0];
    const message = backendMessage || (error.status === 0
      ? 'Unable to reach the server. Please check your connection and try again.'
      : 'Something went wrong. Please try again.');
    return throwError(() => new Error(message));
  }
}
