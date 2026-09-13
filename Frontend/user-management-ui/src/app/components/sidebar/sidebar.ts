import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/authService';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class Sidebar {
  authService = inject(AuthService);
  router = inject(Router);

  isAdmin(): boolean {
    return this.authService.getUser()?.roleName === 'Admin';
  }

  canViewProfile(): boolean {
    const role = this.authService.getUser()?.roleName;
    return role === 'Admin' || role === 'User';
  }

  logout(): void {
    this.authService.logoutUser().subscribe({
      next: () => this.finishLogout(),
      error: () => this.finishLogout()
    });
  }

  private finishLogout(): void {
    this.authService.clearSession();
    this.router.navigate(['/signin']);
  }
}
