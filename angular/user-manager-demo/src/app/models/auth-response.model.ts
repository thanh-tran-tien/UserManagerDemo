export interface AuthResponse {
  token: string;
  expiration: string;
  refreshToken: string;
  refreshTokenExpiration: string;
}
