import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { RoleModel, RolesApiResponse } from '../types/role.type';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  http = inject(HttpClient);
  baseUrl = `https://localhost:7254/api/`;

  getRolesList(): Observable<RoleModel[]>{
    return this.http.get<RolesApiResponse>(this.baseUrl+'Roles').pipe(
      map(response => response.result),
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
