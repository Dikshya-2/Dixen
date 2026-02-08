import { Hall } from "./hall";
import { Ticket } from "./ticket";

export class Booking {
  id: number = 0;
  eventId: number = 0;       
  hallId: number = 0;
  hall?: Hall;
  tickets: Ticket[] = [];
  eventTitle?: string;
  hallName?: string;
  userName?: string;
  bookedTime?: Date;
}
export interface BookingDto {
  id: number;
  eventId: number;
  eventName?: string;
  bookingDate: string;
  status: string;
  userEmail: string;
}

export interface HallCapacityDto {
  hallName: string;
  availableSeats: number;
  totalSeats: number;
}
