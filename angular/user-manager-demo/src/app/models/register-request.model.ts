export interface RegisterRequest {
  user: {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber?: string;
    zipCode?: string;
  };
  password: string;
  confirmPassword: string;
}
