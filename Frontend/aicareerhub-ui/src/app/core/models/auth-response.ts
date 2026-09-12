// auth-response.ts
import { User } from './user';

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: User;
}