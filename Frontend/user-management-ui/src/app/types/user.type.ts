export type UserModel = {
  id: number;
  name: string;
  username: string;
  email: string;
  role: string;
  roleId: number;
}

export type UsersApiResponse = {
  success: boolean;
  message: string;
  result: {
    users: UserModel[]
  };
  errors?: string[];
};

export type UserApiResponse = {
  success: boolean;
  message: string;
  result: UserModel;
  errors?: string[];
};

export type CreateUserPayload = {
  name: string;
  username: string;
  email: string;
  password: string;
  passwordConfirmed: string;
  roleId: number;
};

export type UpdateUserPayload = {
  name: string;
  username: string;
  email: string;
  roleId: number;
  password?: string;
  passwordConfirmed?: string;
};
