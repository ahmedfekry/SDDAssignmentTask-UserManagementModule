import { Routes } from '@angular/router';
import { authGuard } from './guards/authGuard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => {
      return import("./layout/layout").then((m) => m.Layout);
    },
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () => {
          return import("./home/home").then((m) => m.Home);
        },
        title: "Home Page"
      },
      {
        path: 'profile/:id',
        loadComponent: () => {
          return import("./profile/profile").then((m) => m.Profile);
        }
      },
      {
        path: 'users/create',
        loadComponent: () => {
          return import("./users/create-user/create-user").then((m) => m.CreateUser);
        }
      },
      {
        path: 'users/edit/:id',
        loadComponent: () => {
          return import("./users/create-user/create-user").then((m) => m.CreateUser);
        }
      },
      {
        path: 'users',
        loadComponent: () => {
          return import("./users/users").then((m) => m.Users);
        }
      }
    ]
  },
  {
    path: 'signin',
    loadComponent: () => {
      return import("./auth/signin/signin").then((m) => m.Signin);
    },
    title: "Sign In"
  },
  {
    path: "**",
    loadComponent: () => {
      return import("./components/not-found/not-found").then((m) => m.NotFound);
    },
    title: "Not Found"
  }
];
