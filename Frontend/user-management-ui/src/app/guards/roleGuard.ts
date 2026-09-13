import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/authService';

export function roleGuard(allowedRoles: string[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const user = authService.getUser();
    if (!user) {
      return router.createUrlTree(['/signin']);
    }

    return allowedRoles.includes(user.roleName) ? true : router.createUrlTree(['/']);
  };
}
