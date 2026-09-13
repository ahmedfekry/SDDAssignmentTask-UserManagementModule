import { inject, Injectable } from '@angular/core';
import { UserModel } from '../types/user.type';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { ApiResponse } from '../types/ApiResponse.type';


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

  getUsersList(): Observable<UserModel[]>{
    return this.http.get<ApiResponse>(this.baseUrl+'users').pipe(
      map(response => response.result.users),
      catchError(this.handleError)
    );
  }

  deleteUser(userid: number): Observable<ApiResponse>{
    return this.http.delete<ApiResponse>(this.baseUrl+'users/'+userid).pipe(
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
