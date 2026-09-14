import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { UserService } from '../services/userService';
import { AuthService } from '../services/authService';
import { UserModel } from '../types/user.type';

type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-users',
  imports: [RouterLink],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users implements OnInit, OnDestroy {
  usersList = signal<Array<UserModel>>([])
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  page = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = signal(0);

  search = signal('');
  roleFilter = signal('');
  sortBy = signal<string | null>(null);
  sortDirection = signal<SortDirection>('asc');

  private searchInput$ = new Subject<string>();

  userService = inject(UserService);
  authService = inject(AuthService);

  isAdmin(): boolean {
    return this.authService.getUser()?.roleName === 'Admin';
  }

  ngOnInit(): void {
    this.searchInput$.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe((value) => {
      this.search.set(value);
      this.page.set(1);
      this.loadUsersData();
    });

    this.loadUsersData();
  }

  ngOnDestroy(): void {
    this.searchInput$.complete();
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
    .getUsersList(
      this.page(),
      this.pageSize(),
      this.search() || undefined,
      this.roleFilter() || undefined,
      this.sortBy() || undefined,
      this.sortDirection()
    )
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

  onSearchInput(event: Event): void {
    this.searchInput$.next((event.target as HTMLInputElement).value);
  }

  onRoleFilterChange(event: Event): void {
    this.roleFilter.set((event.target as HTMLSelectElement).value);
    this.page.set(1);
    this.loadUsersData();
  }

  sortByColumn(key: string): void {
    if (this.sortBy() === key) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortBy.set(key);
      this.sortDirection.set('asc');
    }
    this.page.set(1);
    this.loadUsersData();
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
