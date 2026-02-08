import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Booking } from '../../Models/booking';
import { Hall } from '../../Models/hall';
import { GenericService } from '../../Services/generic-service';

@Component({
  selector: 'app-booking-component',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './booking-component.html',
  styleUrl: './booking-component.css',
})
export class BookingComponent {
  bookings: Booking[] = [];
  newBooking: Booking = new Booking();
  halls: Hall[] = [];
  showBookingForm: boolean = false;
  selectedBooking: Booking | null = null;
  loading: boolean = false;

  constructor(
    private bookingService: GenericService<any>,
    private hallService: GenericService<Hall>,
  ) {}

  ngOnInit(): void {
    this.loadBookings();
    this.loadHalls();
  }

  get bookingToEditOrCreate(): Booking {
    // Always return a non-null Booking
    return this.selectedBooking ?? this.newBooking;
  }

  toggleForm(): void {
    this.showBookingForm = !this.showBookingForm;
    this.selectedBooking = null;
    this.newBooking = new Booking();
  }

  selectBooking(booking: Booking): void {
    this.selectedBooking = { ...booking };
    this.showBookingForm = true;
  }

  createBooking(): void {
    const booking = this.bookingToEditOrCreate;

    if (!booking.eventId || !booking.hallId) {
      alert('Please select an event and hall.');
      return;
    }

    this.loading = true;

    this.bookingService
      // .post(`Booking/${booking.eventId}`, booking)
      .post(`Booking/${this.newBooking.eventId}`, {
        hallId: this.newBooking.hallId,
      })

      .subscribe({
        next: () => {
          this.loadBookings();
          this.toggleForm();
          this.loading = false;
        },
        error: (err) => {
          console.error('Error creating booking:', err);
          this.loading = false;
        },
      });
  }

  cancelBooking(id: number): void {
    if (!confirm('Are you sure you want to cancel this booking?')) return;

    this.bookingService.delete('Booking', id).subscribe({
      next: () => this.loadBookings(),
      error: (err) => console.error('Error cancelling booking:', err),
    });
  }

  loadBookings(): void {
    this.bookingService.getAll('Booking/all').subscribe({
      next: (data: Booking[]) => (this.bookings = data),
      error: (err) => console.error(err),
    });
  }

  loadHalls(): void {
    this.hallService.getAll('Hall').subscribe({
      next: (data: Hall[]) => (this.halls = data),
      error: (err) => console.error(err),
    });
  }
}
