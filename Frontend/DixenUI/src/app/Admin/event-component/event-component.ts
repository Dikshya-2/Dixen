import { Component } from '@angular/core';
import { Evnt } from '../../Models/Evnt';
import { GenericService } from '../../Services/generic-service';
import { Category } from '../../Models/category';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormControl, FormGroup, FormsModule, Validators } from '@angular/forms';
import { Hall } from '../../Models/hall';
import { Organizer } from '../../Models/organizer';

@Component({
  selector: 'app-event-component',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './event-component.html',
  styleUrl: './event-component.css',
})
export class EventComponent {
   events: Evnt[] = [];
  categories: Category[] = [];
  halls: Hall[] = [];
  organizers: Organizer[] = [];

  newEvent: Evnt = new Evnt();
  selectedEvent: Evnt | null = null;
  showCreateForm = false;

  constructor(
    private eventService: GenericService<Evnt>,
    private categoryService: GenericService<Category>,
    private hallService: GenericService<Hall>,
    private organizerService: GenericService<Organizer>,
  ) {}

  ngOnInit(): void {
    this.loadEvents();
    this.loadCategories();
    this.loadHalls();
    this.loadOrganizers();
  }

  loadEvents() {
    this.eventService.getAll('Event').subscribe(a => this.events = a);
  }

  loadCategories() {
    this.categoryService.getAll('Category').subscribe(a => this.categories = a);
  }

  loadHalls() {
    this.hallService.getAll('Hall').subscribe(a => this.halls = a);
  }

  loadOrganizers() {
    this.organizerService.getAll('Organizer').subscribe(a => this.organizers = a);
  }

  toggleCreateForm() {
  if (this.showCreateForm) {
    this.showCreateForm = false;
    this.selectedEvent = null;
    this.newEvent = { 
      ...new Evnt(), 
      categoryIds: [], 
      hallIds: [] 
    };
  } else {
    this.showCreateForm = true;
    if (!this.selectedEvent) {
      this.newEvent = { 
        ...new Evnt(), 
        categoryIds: [], 
        hallIds: [] 
      };
    }
  }
}
  selectCategory(id: number) {
    const index = this.newEvent.categoryIds.indexOf(id);
    if (index > -1) this.newEvent.categoryIds.splice(index, 1);
    else this.newEvent.categoryIds.push(id);
  }

  selectHall(id: number) {
    const index = this.newEvent.hallIds.indexOf(id);
    if (index > -1) this.newEvent.hallIds.splice(index, 1);
    else this.newEvent.hallIds.push(id);
  }

  saveEvent() {
    if (!this.newEvent.title || !this.newEvent.startTime || !this.newEvent.organizerId) {
      alert('Title, Start Time, and Organizer are required.');
      return;
    }

    if (this.selectedEvent?.id) {
      this.eventService.put('Event', this.selectedEvent.id, this.newEvent).subscribe(() => {
        this.loadEvents();
        this.cancelEdit();
      });
    } else {
      this.eventService.post('Event', this.newEvent).subscribe(() => {
        this.loadEvents();
        this.toggleCreateForm();
      });
    }
  }

  editEvent(ev: Evnt) {
  this.selectedEvent = { ...ev };
  this.newEvent = { 
    ...ev, 
    categoryIds: ev.categoryIds || [],
    hallIds: ev.hallIds || []
  };
  this.showCreateForm = true;
}

  cancelEdit() {
    this.selectedEvent = null;
    this.newEvent = new Evnt();
    this.showCreateForm = false;
  }

  deleteEvent(id: number) {
    if (!confirm('Are you sure?')) return;
    this.eventService.delete('Event', id).subscribe(() => this.loadEvents());
  }
}
