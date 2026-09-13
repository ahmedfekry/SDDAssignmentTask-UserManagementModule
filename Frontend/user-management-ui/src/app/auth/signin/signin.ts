import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../services/authService';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signin',
  imports: [ReactiveFormsModule],
  templateUrl: './signin.html',
  styleUrl: './signin.css',
})
export class Signin {

  submitting = signal(false);
  errorMessage = signal<string | null>(null);
  submitted = false;

  authService = inject(AuthService);
  formBuilder = inject(FormBuilder);
  router = inject(Router);

  form = this.formBuilder.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, password } = this.form.getRawValue();
    this.submitting.set(true);

    this.authService.loginUser({ username: username!, password: password! }).subscribe({
      next: (data) => {
        this.submitting.set(false);
        if (data.success) {
          this.authService.setSession({
            userId: data.result.userId,
            username: data.result.username,
            roleName: data.result.roleName
          });
          this.router.navigate(['/']);
        } else {
          this.errorMessage.set(data.message);
        }
      },
      error: (err: Error) => {
        this.submitting.set(false);
        this.errorMessage.set(err.message);
      }
    });
  }
}
