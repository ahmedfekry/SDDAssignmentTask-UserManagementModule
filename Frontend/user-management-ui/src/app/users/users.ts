import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserService } from '../services/userService';
import { UserModel } from '../types/user.type';
@Component({
  selector: 'app-users',
  imports: [RouterLink],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users implements OnInit {
  usersList = signal<Array<UserModel>>([])
  userService = inject(UserService);

  ngOnInit(): void {
    this.loadUsersData();
  }

  deleteUser(userId: number){
    this.userService
    .deleteUser(userId)
    .subscribe((data) => {
      console.log(data);
      if(data.success == true){
        this.loadUsersData();
      }
    });
    // alert(userId);
  }

  loadUsersData(): void{
    this.userService
    .getUsersList()
    .subscribe((users) => {
      this.usersList.set(users);
    })
  }
}


