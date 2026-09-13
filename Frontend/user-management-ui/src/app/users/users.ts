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
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  userService = inject(UserService);

  ngOnInit(): void {
    this.loadUsersData();
  }

  deleteUser(userId: number){
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.userService
    .deleteUser(userId)
    .subscribe({
      next: (data) => {
        if (data.success) {
          this.successMessage.set(data.message);
          this.loadUsersData();
        } else {
          this.errorMessage.set(data.message);
        }
      },
      error: (err: Error) => {
        this.errorMessage.set(err.message);
      }
    });
  }

  loadUsersData(): void{
    this.errorMessage.set(null);
    this.userService
    .getUsersList()
    .subscribe({
      next: (users) => {
        this.usersList.set(users);
      },
      error: (err: Error) => {
        this.errorMessage.set(err.message);
      }
    });
  }
}


