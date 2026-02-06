export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  gender: string;
  age: number;
  roles: string[];         
  is2FAEnabled: boolean;
  attendedEvents: string[];
  hostedEvents: string[];
  proposedEvents: string[];
  preferredCategories: string[];
}
