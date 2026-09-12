import { Injectable } from '@angular/core';
import { UserModel } from '../types/user.type';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  users: Array<UserModel> = [
    {
      Id: 1,
      UserName: "ahmedfekry",
      Email: "ahmedfikr@maiul.com",
      Role: "Admin",
      Name: "Ahmed fEkrt",
      RoleId: 1
    },
    {
      Id: 2,
      UserName: "Alymohamed",
      Email: "alymohamed@maiul.com",
      Role: "Admin",
      Name: "aly fEkrt",
      RoleId: 1
    },
    {
      Id: 3,
      UserName: "Kareem",
      Email: "kemoi@maiul.com",
      Role: "Admin",
      Name: "Kareem fEkrt",
      RoleId: 1
    }
  ]

}
