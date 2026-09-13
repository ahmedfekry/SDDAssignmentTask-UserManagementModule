export type RoleModel = {
  id: number;
  name: string;
  description: string;
};

export type RolesApiResponse = {
  success: boolean;
  message: string;
  result: RoleModel[];
  errors?: string[];
};
