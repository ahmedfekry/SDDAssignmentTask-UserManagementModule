import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { authInterceptor } from './interceptors/authInterceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    // Angular's own withXsrfConfiguration() only attaches the XSRF header to
    // same-origin/relative requests - our API is on a different origin, so
    // authInterceptor attaches X-XSRF-TOKEN manually instead.
    provideHttpClient(
      withFetch(),
      withInterceptors([authInterceptor])
    )
  ]
};
