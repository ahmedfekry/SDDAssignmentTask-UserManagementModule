import { Component, inject, OnInit, signal } from '@angular/core';
import { RoleService } from '../../services/roleService';
import { RoleModel } from '../../types/role.type';

@Component({
  selector: 'app-create-user',
  imports: [],
  templateUrl: './create-user.html',
  styleUrl: './create-user.css',
})
export class CreateUser implements OnInit {
  roles = signal<RoleModel[]>([]);
  rolesError = signal<string | null>(null);
  roleService = inject(RoleService);

  ngOnInit(): void {
    this.roleService.getRolesList().subscribe({
      next: (roles) => this.roles.set(roles),
      error: (err: Error) => this.rolesError.set(err.message)
    });
  }
}
