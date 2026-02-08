// import { Venue } from "./venue";

// export class Hall {
//   id: number = 0;
//   name: string = "";
//   capacity: number = 0;
//   eventId: number = 0;
//   // attendanceCount?: number;
//   venueId: number = 0;
// }
import { Evnt } from "./Evnt";
import { Venue } from "./venue";

export class Hall {
  id: number = 0;
  name: string = "";
  capacity: number = 0;
  eventId: number = 0;
  event?: Evnt;        
  venueId: number = 0;
  venue?: Venue;  
  seatsAvailable: number = 0;     
  isDeleted: boolean = false;
}
