import { inject, Injectable } from '@angular/core';
import { CreateUserPayload, PagedUsers, UpdateUserPayload, UserApiResponse, UserModel } from '../types/user.type';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { UsersApiResponse } from '../types/user.type';


@Injectable({
  providedIn: 'root',
})

export class UserService {
  // users: Array<UserModel> = [
  //   {
  //     Id: 1,
  //     UserName: "ahmedfekry",
  //     Email: "ahmedfikr@maiul.com",
  //     Role: "Admin",
  //     Name: "Ahmed fEkrt",
  //     RoleId: 1
  //   },
  //   {
  //     Id: 2,
  //     UserName: "Alymohamed",
  //     Email: "alymohamed@maiul.com",
  //     Role: "Admin",
  //     Name: "aly fEkrt",
  //     RoleId: 1
  //   },
  //   {
  //     Id: 3,
  //     UserName: "Kareem",
  //     Email: "kemoi@maiul.com",
  //     Role: "Admin",
  //     Name: "Kareem fEkrt",
  //     RoleId: 1
  //   }
  // ];

  http = inject(HttpClient);
  baseUrl = `https://localhost:7254/api/`;

  getUsersList(page: number = 1, pageSize: number = 8): Observable<PagedUsers>{
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<UsersApiResponse>(this.baseUrl+'users', { params }).pipe(
      map(response => response.result),
      catchError(this.handleError)
    );
  }

  deleteUser(userid: number): Observable<UsersApiResponse>{
    return this.http.delete<UsersApiResponse>(this.baseUrl+'users/'+userid).pipe(
      catchError(this.handleError)
    );
  }

  createUser(payload: CreateUserPayload): Observable<UsersApiResponse>{
    return this.http.post<UsersApiResponse>(this.baseUrl+'users', payload).pipe(
      catchError(this.handleError)
    );
  }

  getUserById(userId: number): Observable<UserModel>{
    return this.http.get<UserApiResponse>(this.baseUrl+'users/'+userId).pipe(
      map(response => response.result),
      catchError(this.handleError)
    );
  }

  updateUser(userId: number, payload: UpdateUserPayload): Observable<UsersApiResponse>{
    return this.http.put<UsersApiResponse>(this.baseUrl+'users/'+userId, payload).pipe(
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
