import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookingDto, HallCapacityDto } from '../../Models/booking';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-booking-detail-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-detail-component.html',
  styleUrl: './booking-detail-component.css',
})
export class BookingDetailComponent {
   eventId: number = 0;
  availability: HallCapacityDto[] = [];
  isLoading = false;
  bookingSuccess = false;
  errorMessage = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {}

  async checkAvailability() {
    if (!this.eventId) return;
    
    this.isLoading = true;
    try {
      const url = `https://your-api.com/api/booking/availability/${this.eventId}`;
      this.availability = await this.http.get<HallCapacityDto[]>(url).toPromise() || [];
    } catch (error) {
      this.errorMessage = 'Failed to load availability';
    } finally {
      this.isLoading = false;
    }
  }

  async createBooking() {
    if (!this.eventId) {
      this.errorMessage = 'Please enter event ID';
      return;
    }

    this.isLoading = true;
    try {
      const url = `https://your-api.com/api/booking/${this.eventId}`;
      const booking = await this.http.post<BookingDto>(url, {}).toPromise();
      
      if (booking) {
        this.bookingSuccess = true;
        // Optionally navigate to tickets or reload bookings
      }
    } catch (error: any) {
      this.errorMessage = error.error || 'Booking failed';
    } finally {
      this.isLoading = false;
    }
  }
}
