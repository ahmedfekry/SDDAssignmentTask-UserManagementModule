import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/authService';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  authService = inject(AuthService);

  username(): string {
    return this.authService.getUser()?.username ?? '';
  }

  roleName(): string {
    return this.authService.getUser()?.roleName ?? '';
  }

  avatarInitials(): string {
    return this.username().slice(0, 2).toUpperCase();
  }
}
