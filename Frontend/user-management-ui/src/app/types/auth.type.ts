export type Token = {
  token: string;
  userid: number;
  username: string;
  roleName: string;
}

export type AuthApiResponse = {
  success: boolean;
  message: string;
  result: Token;
  errors?: string[];
};


export type AuthPayload = {
  username: string;
  password: string;
}
