import { Component } from '@angular/core';
import { Hall } from '../../Models/hall';
import { GenericService } from '../../Services/generic-service';
import { Evnt } from '../../Models/Evnt';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-hall-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './hall-component.html',
  styleUrl: './hall-component.css',
})
export class HallComponent {
  halls: Hall[] = [];
  events: Evnt[] = [];

  newHall: Hall = new Hall();
  selectedHall: Hall | null = null;
  showForm: boolean = false;

  constructor(
    private hallService: GenericService<Hall>,
    private eventService: GenericService<Evnt>
  ) {}

  ngOnInit(): void {
    this.loadHalls();
    this.loadEvents();
  }

  loadHalls() {
    this.hallService.getAll('Hall').subscribe(h => (this.halls = h));
  }

  loadEvents() {
    this.eventService.getAll('Event').subscribe(e => (this.events = e));
  }

  toggleForm() {
    this.showForm = !this.showForm;
    this.newHall = new Hall();
    this.selectedHall = null;
  }

  editHall(hall: Hall) {
    this.selectedHall = { ...hall };
    this.newHall = { ...hall };
    this.showForm = true;
  }

  cancelEdit() {
    this.selectedHall = null;
    this.newHall = new Hall();
    this.showForm = false;
  }

  saveHall() {
    if (!this.newHall.name || !this.newHall.capacity || !this.newHall.eventId) {
      alert('Name, Capacity, and Event are required');
      return;
    }

    const request$ = this.selectedHall?.id
      ? this.hallService.put('Hall', this.selectedHall.id, this.newHall)
      : this.hallService.post('Hall', this.newHall);

    request$.subscribe(() => {
      this.loadHalls();
      this.selectedHall ? this.cancelEdit() : this.toggleForm();
    });
  }

  deleteHall(id: number) {
    if (!confirm('Are you sure you want to delete this hall?')) return;
    this.hallService.delete('Hall', id).subscribe(() => this.loadHalls());
  }

  getEventTitle(eventId: number): string {
    const ev = this.events.find(e => e.id === eventId);
    return ev ? ev.title : '';
  }
}
