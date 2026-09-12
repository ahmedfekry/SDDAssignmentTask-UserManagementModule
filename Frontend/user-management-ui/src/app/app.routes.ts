import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => {
      return import("./home/home").then((m) => m.Home);
    }
  },
  {
    path: 'signin',
    pathMatch: 'full',
    loadComponent: () => {
      return import("./auth/signin/signin").then((m) => m.Signin);
    }
  }
];
