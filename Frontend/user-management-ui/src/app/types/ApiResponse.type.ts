import { UserModel } from "./user.type";

export type ApiResponse = {
  success: boolean;
  message: string;
  result: {
    users: UserModel[]
  }
};
