export class Ticket {
  id?: number;
  type!: number;      
  price!: number;
  quantity!: number;
}
export interface TicketCreateDto {
  type: number;
  quantity: number;
}
