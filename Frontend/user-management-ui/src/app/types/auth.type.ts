export type LoginResult = {
  accessToken: string;
  expiresIn: number;
  userId: number;
  username: string;
  roleName: string;
}

export type AuthApiResponse = {
  success: boolean;
  message: string;
  result: LoginResult;
  errors?: string[];
};


export type AuthPayload = {
  username: string;
  password: string;
}

export type AuthUser = {
  userId: number;
  username: string;
  roleName: string;
}
