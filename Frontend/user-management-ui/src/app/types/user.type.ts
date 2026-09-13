export type UserModel = {
  id: number;
  name: string;
  username: string;
  email: string;
  role: string;
  roleid: number;
}

export type UsersApiResponse = {
  success: boolean;
  message: string;
  result: {
    users: UserModel[]
  };
  errors?: string[];
};
