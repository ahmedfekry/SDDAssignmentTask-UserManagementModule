import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => {
      return import("./home/home").then((m) => m.Home);
    },
    title: "Home Page"
  },
  {
    path: 'signin',
    loadComponent: () => {
      return import("./auth/signin/signin").then((m) => m.Signin);
    },
    title: "Sign In"
  },
  {
    path: 'profile/:id',
    loadComponent: () => {
      return import("./profile/profile").then((m) => m.Profile);
    }
  },
  {
    path: "**",
    loadComponent: () => {
      return import("./components/not-found/not-found").then((m) => m.NotFound);
    },
    title: "Not Found"
  }
];
