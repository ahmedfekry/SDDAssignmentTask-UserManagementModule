import { ApplicationConfig, inject, PLATFORM_ID, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { catchError, firstValueFrom, of } from 'rxjs';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { jwtInterceptor } from './interceptors/jwtInterceptor';
import { AuthService } from './services/authService';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(
      withFetch(),
      withInterceptors([jwtInterceptor])
    ),
    // Restores the session silently on app boot (page load/hard refresh) using the
    // HttpOnly refresh-token cookie, before any guard/component runs. Skipped during
    // SSR/prerender - there's no browser cookie jar in Node, and the only prerendered
    // pages (signin, not-found) are public anyway.
    provideAppInitializer(() => {
      if (!isPlatformBrowser(inject(PLATFORM_ID))) {
        return Promise.resolve();
      }
      const authService = inject(AuthService);
      return firstValueFrom(authService.refreshToken().pipe(catchError(() => of(false))));
    })
  ]
};
