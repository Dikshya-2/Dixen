export class Ticket {
  id?: number;
  type!: number;      // enum/int
  price!: number;
  quantity!: number;
}
// ticket-create.dto.ts
export interface TicketCreateDto {
  type: number;
  quantity: number;
}
