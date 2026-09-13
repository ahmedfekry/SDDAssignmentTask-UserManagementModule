import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserService } from '../services/userService';
import { AuthService } from '../services/authService';
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

  page = signal(1);
  pageSize = signal(8);
  totalCount = signal(0);
  totalPages = signal(0);

  userService = inject(UserService);
  authService = inject(AuthService);

  isAdmin(): boolean {
    return this.authService.getUser()?.roleName === 'Admin';
  }

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
    .getUsersList(this.page(), this.pageSize())
    .subscribe({
      next: (data) => {
        // The page we asked for no longer exists (e.g. the last item on it was just
        // deleted) - fall back to the new last page instead of showing an empty table.
        if (data.totalPages > 0 && this.page() > data.totalPages) {
          this.page.set(data.totalPages);
          this.loadUsersData();
          return;
        }

        this.usersList.set(data.users);
        this.totalCount.set(data.totalCount);
        this.totalPages.set(data.totalPages);
      },
      error: (err: Error) => {
        this.errorMessage.set(err.message);
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) {
      return;
    }
    this.page.set(page);
    this.loadUsersData();
  }

  onPageSizeChange(event: Event): void {
    this.pageSize.set(Number((event.target as HTMLSelectElement).value));
    this.page.set(1);
    this.loadUsersData();
  }

  pageNumbers(): number[] {
    return Array.from({ length: this.totalPages() }, (_, i) => i + 1);
  }

  rangeStart(): number {
    return this.totalCount() === 0 ? 0 : (this.page() - 1) * this.pageSize() + 1;
  }

  rangeEnd(): number {
    return Math.min(this.page() * this.pageSize(), this.totalCount());
  }
}


