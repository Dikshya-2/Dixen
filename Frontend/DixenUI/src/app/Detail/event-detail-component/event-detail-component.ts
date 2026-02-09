import { Component } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Evnt } from '../../Models/Evnt';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { Venue } from '../../Models/venue';
import { Booking,  } from '../../Models/booking';
import { Ticket, TicketCreateDto } from '../../Models/ticket';
import { FormsModule } from '@angular/forms';
import { Hall } from '../../Models/hall';
import { HallCapacityDto } from '../../Models/HallCapacityDto';

@Component({
  selector: 'app-event-detail-component',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './event-detail-component.html',
  styleUrl: './event-detail-component.css',
})
export class EventDetailComponent {
  eventId!: number;
  event!: Evnt;
  halls: Hall[] = [];
  loading: boolean = true;
  bookingId: number | null = null;
  tickets: Ticket[] = [];
  selectedHallId: number = 0;

  ticketPrices: Record<number, number> = { 1: 25, 2: 50, 3: 15 };
  ticketNames: Record<number, string> = {
    1: 'General',
    2: 'VIP',
    3: 'Student',
  };

  bookingStep: 'selection' | 'payment' | 'success' = 'selection';
  selectedTicketType: number = 1;
  quantity: number = 1;

  constructor(
    private route: ActivatedRoute,
    private eventService: GenericService<Evnt>,
    private genericService: GenericService<any>,
    private bookingService: GenericService<any>,
    private ticketService: GenericService<Ticket, TicketCreateDto>,
  ) {}

  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.eventService.getById('Event', this.eventId).subscribe({
      next: (event: Evnt) => {
        this.event = event;
        console.log('Event loaded:', event);
      },
    });
    this.refreshAvailability();
  }
  private refreshAvailability(): void {
    this.genericService
      .getAny<any[]>('Booking/availability/' + this.eventId)
      .subscribe({
        next: (availability: HallCapacityDto[]) => {
          this.halls = availability.map((item: any) => ({
            id: item.hallId,
            name: item.hallName,
            capacity: item.capacity,
            seatsAvailable: item.seatsAvailable,
            eventId: this.eventId,
            venueId: 1,
            isDeleted: false,
          }));
          console.log('REAL HALLS:', this.halls);
          this.loading = false;
        },
        error: (err: any) => {
          console.error('Halls failed:', err);
          this.loading = false;
        },
      });
  }
  processPayment(): void {
    if (!this.selectedHallId || this.selectedHallId === 0) {
      alert('Please select a hall!');
      return;
    }
    this.bookingService.post('Booking/' + this.eventId, {}).subscribe({
      next: (response: any) => {
        this.bookingId = response.id;
        this.bookingStep = 'payment';
        setTimeout(() => this.createTickets(), 2000);
      },
      error: (err: any) =>
        alert('Booking failed: ' + (err.error || 'Unknown error')),
    });
  }
  private createTickets(): void {
    if (!this.bookingId) return;

    const dto: TicketCreateDto = {
      type: this.selectedTicketType,
      quantity: this.quantity,
    };
    this.ticketService.post('Ticket/booking/' + this.bookingId, dto).subscribe({
      next: (response: any) => {
        this.tickets = Array.isArray(response) ? response : [response];

        setTimeout(() => {
          this.refreshAvailability();
          this.bookingStep = 'success';
        }, 1000);
      },
      error: (err: any) => {
        alert('Ticket creation failed: ' + (err.error || 'Unknown error'));
        this.bookingStep = 'payment';
      },
    });
  }
  getTotalPrice(): number {
    return (this.ticketPrices[this.selectedTicketType] || 0) * this.quantity;
  }

  getTicketTypeName(type: number): string {
    return this.ticketNames[type] || 'Unknown';
  }
  getGoogleMapsLink(): string {
    if (!this.event) return '#';
    const venueName = this.event.name?.trim() || '';
    const address = this.event.address?.trim() || '';
    const city = this.event.city?.trim() || '';
    const query = [venueName, address, city]
      .filter((part) => part.length > 0)
      .join(', ');
    return `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(query)}`;
  }
  get eventName(): string {
    return this.event?.name || 'Venue';
  }
  get eventAddress(): string {
    return this.event?.address || '';
  }
  get eventCity(): string {
    return this.event?.city || '';
  }

  getAvailableHalls(): Hall[] {
    return this.halls.filter((h) => h.seatsAvailable > 0 && !h.isDeleted);
  }

  getSelectedHallName(): string {
    if (!this.selectedHallId || this.halls.length === 0) return 'Select Hall';
    const hall = this.halls.find((h) => h.id === this.selectedHallId);
    return hall ? `${hall.name} (${hall.capacity} seats)` : 'Select Hall';
  }

  resetBooking(): void {
    this.bookingStep = 'selection';
    this.selectedHallId = 0;
    this.selectedTicketType = 1;
    this.quantity = 1;
    this.bookingId = null;
    this.tickets = [];
  }
}
