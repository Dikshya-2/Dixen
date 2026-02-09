import { Booking } from "./booking";

export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  gender: string;
  age: number;
  roles: string[];         
  is2FAEnabled: boolean;
   bookings?: Booking[];
  // bookings: string[] | Booking[]; 
  hostedEvents: string[];
  proposedEvents: string[];
  preferredCategories: string[];
}
