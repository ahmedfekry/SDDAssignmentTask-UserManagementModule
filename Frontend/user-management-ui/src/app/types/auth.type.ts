export type Token = {
  jwtToken: JwtToken;
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

export type JwtToken = {
  token: string;
  expiresAt: string;
}

export type AuthPayload = {
  username: string;
  password: string;
}
