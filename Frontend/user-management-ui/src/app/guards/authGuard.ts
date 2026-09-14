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

  // No access token in memory - most likely a fresh page load/hard refresh, since the
  // token is intentionally never persisted. Try a silent refresh via the HttpOnly
  // refresh-token cookie before concluding the user is actually logged out.
  return authService.refreshAccessToken().pipe(
    map((refreshed) => refreshed ? true : router.createUrlTree(['/signin']))
  );
};
