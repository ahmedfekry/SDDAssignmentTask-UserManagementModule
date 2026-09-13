import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/authService';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);

  if (!isPlatformBrowser(inject(PLATFORM_ID))) {
    return router.createUrlTree(['/signin']);
  }

  const authService = inject(AuthService);
  return authService.isAuthenticated() ? true : router.createUrlTree(['/signin']);
};
