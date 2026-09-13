import { Component, OnInit, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../services/authService';
import { UserService } from '../services/userService';

function passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirmPassword = group.get('passwordConfirmed')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile implements OnInit {
  loadingUser = signal(true);
  submitting = signal(false);
  submitted = false;
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  roleName = signal<string | null>(null);

  private userId!: number;
  private roleId!: number;

  route = inject(ActivatedRoute);
  userService = inject(UserService);
  authService = inject(AuthService);
  formBuilder = inject(FormBuilder);

  form = this.formBuilder.group({
    name: ['', Validators.required],
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.minLength(8)],
    passwordConfirmed: [''],
  }, { validators: passwordsMatchValidator });

  ngOnInit(): void {
    this.userId = Number(this.route.snapshot.paramMap.get('id'));

    this.userService.getUserById(this.userId).subscribe({
      next: (user) => {
        this.loadingUser.set(false);
        this.roleId = user.roleId;
        this.roleName.set(user.role);
        this.form.patchValue({
          name: user.name,
          username: user.username,
          email: user.email,
        });
      },
      error: (err: Error) => {
        this.loadingUser.set(false);
        this.errorMessage.set(err.message);
      }
    });
  }

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, username, email, password, passwordConfirmed } = this.form.getRawValue();
    this.submitting.set(true);

    this.userService
      .updateUser(this.userId, {
        name: name!,
        username: username!,
        email: email!,
        roleId: this.roleId,
        ...(password ? { password, passwordConfirmed: passwordConfirmed! } : {})
      })
      .subscribe({
        next: (data) => {
          this.submitting.set(false);
          if (data.success) {
            this.successMessage.set(data.message);
            this.form.patchValue({ password: '', passwordConfirmed: '' });
            this.form.markAsPristine();

            const currentUser = this.authService.getUser();
            if (currentUser?.userId === this.userId) {
              this.authService.setSession({ ...currentUser, username: username! });
            }
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
