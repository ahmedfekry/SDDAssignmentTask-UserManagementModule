import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserService } from '../services/userService';
import { UserModel } from '../types/user.type';
import { catchError } from 'rxjs';
import { error } from 'console';
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
    // this.usersList.set(this.userService.users);
    this.userService
      .getUsersList()
      .subscribe((data) => {
        console.log(data.result);
      });
    // this.usersList.set(usersList)

  }
}
