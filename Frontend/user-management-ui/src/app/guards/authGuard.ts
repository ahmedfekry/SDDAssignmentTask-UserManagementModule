import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../services/authService';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);

  // Guarded routes are rendered client-side only (see app.routes.server.ts), so this
  // should never actually run during SSR/prerender - but bail out safely if it ever does.
  if (!isPlatformBrowser(inject(PLATFORM_ID))) {
    return router.createUrlTree(['/signin']);
  }

  const authService = inject(AuthService);

  if (authService.getAccessToken()) {
    return true;
  }

  // The app initializer already attempted a silent refresh at boot. If there's still no
  // token (e.g. it expired mid-session), try once more before giving up.
  return authService.refreshToken().pipe(
    map((refreshed) => refreshed ? true : router.createUrlTree(['/signin']))
  );
};
