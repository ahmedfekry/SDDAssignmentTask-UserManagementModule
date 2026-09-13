import { Component, inject, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { RoleService } from '../../services/roleService';
import { RoleModel } from '../../types/role.type';
import { UserService } from '../../services/userService';

function passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirmPassword = group.get('passwordConfirmed')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-create-user',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-user.html',
  styleUrl: './create-user.css',
})

export class CreateUser implements OnInit {
  roles = signal<RoleModel[]>([]);
  rolesError = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  submitting = signal(false);
  submitted = false;

  roleService = inject(RoleService);
  userService = inject(UserService);
  router = inject(Router);
  formBuilder = inject(FormBuilder);

  form = this.formBuilder.group({
    name: ['', Validators.required],
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    passwordConfirmed: ['', Validators.required],
    roleId: [null as number | null, Validators.required],
  }, { validators: passwordsMatchValidator });

  ngOnInit(): void {
    this.roleService.getRolesList().subscribe({
      next: (roles) => this.roles.set(roles),
      error: (err: Error) => this.rolesError.set(err.message)
    });
  }

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, username, email, password,passwordConfirmed, roleId } = this.form.getRawValue();
    this.submitting.set(true);
    this.userService
      .createUser({ name: name!,
                    username: username!,
                    email: email!,
                    password: password!,
                    passwordConfirmed: passwordConfirmed!,
                    roleId: roleId! })
      .subscribe({
        next: (data) => {
          this.submitting.set(false);
          if (data.success) {
            this.router.navigate(['/users']);
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
