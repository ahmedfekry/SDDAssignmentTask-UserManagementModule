import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Signin } from './componenets/auth/signin/signin';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,Signin],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('User Management Module');
}
