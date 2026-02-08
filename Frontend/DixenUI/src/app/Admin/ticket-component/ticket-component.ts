import { Component } from '@angular/core';
import { Ticket } from '../../Models/ticket';
import { GenericService } from '../../Services/generic-service';
import { CommonModule, formatCurrency } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-ticket-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-component.html',
  styleUrl: './ticket-component.css',
})
export class TicketComponent {
  tickets: Ticket[] = [];

  bookingId!: number;
  newTicket: Ticket = new Ticket();
  selectedTicket: Ticket | null = null;
  showForm = false;

  constructor(private ticketService: GenericService<Ticket>) {}

  loadTickets() {
    if (!this.bookingId) return;

    this.ticketService
      .getAll(`Ticket/booking/${this.bookingId}`)
      .subscribe(res => (this.tickets = res));
  }

  toggleForm() {
    this.showForm = !this.showForm;

    if (!this.showForm) {
      this.cancelEdit();
    } else if (!this.selectedTicket) {
      this.newTicket = new Ticket();
    }
  }

  edit(t: Ticket) {
    this.selectedTicket = { ...t };
    this.newTicket = { ...t };
    this.showForm = true;
  }

  save() {
    if (!this.newTicket.type || !this.newTicket.price || !this.newTicket.quantity) {
      alert('All fields are required');
      return;
    }

    if (this.selectedTicket?.id) {
      this.ticketService
        .put('Ticket', this.selectedTicket.id, this.newTicket)
        .subscribe(() => {
          this.loadTickets();
          this.cancelEdit();
        });
    } else {
      this.ticketService
        .post(`Ticket/booking/${this.bookingId}`, this.newTicket)
        .subscribe(() => {
          this.loadTickets();
          this.toggleForm();
        });
    }
  }

  cancelEdit() {
    this.selectedTicket = null;
    this.newTicket = new Ticket();
    this.showForm = false;
  }

  delete(id: number) {
    if (!confirm('Delete ticket?')) return;

    this.ticketService
      .delete('Ticket', id)
      .subscribe(() => this.loadTickets());
  }

}
