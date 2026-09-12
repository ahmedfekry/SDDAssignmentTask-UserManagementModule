import { inject, Injectable } from '@angular/core';
import { UserModel } from '../types/user.type';
import { HttpClient } from '@angular/common/http';
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

  getUsersList(){
    const url = `https://localhost:7254/api/Users`;
    // return this.http.get<Array<UserModel>>(url);
    return this.http.get<ApiResponse>(url);
  }

}
