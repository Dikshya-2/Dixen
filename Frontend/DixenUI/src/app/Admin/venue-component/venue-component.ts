import { Component } from '@angular/core';
import { Venue } from '../../Models/venue';
import { GenericService } from '../../Services/generic-service';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-venue-component',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './venue-component.html',
  styleUrl: './venue-component.css',
})
export class VenueComponent {
   venues: Venue[] = [];
  newVenue: Venue = new Venue();
  selectedVenue: Venue | null = null;
  showForm: boolean = false;

  constructor(private venueService: GenericService<Venue>) {}

  ngOnInit(): void {
    this.loadVenues();
  }

  loadVenues() {
    this.venueService.getAll('Venue').subscribe(v => this.venues = v);
  }

  toggleForm() {
    this.showForm = !this.showForm;
    this.newVenue = new Venue();
    this.selectedVenue = null;
  }

  editVenue(venue: Venue) {
    this.selectedVenue = { ...venue };
    this.newVenue = { ...venue };
    this.showForm = true;
  }

  saveVenue() {
    if (!this.newVenue.name || !this.newVenue.address || !this.newVenue.city) {
      alert('All fields are required.');
      return;
    }

    if (this.selectedVenue?.id) {
      this.venueService.put('Venue', this.selectedVenue.id, this.newVenue)
        .subscribe(() => {
          this.loadVenues();
          this.cancelEdit();
        });
    } else {
      this.venueService.post('Venue', this.newVenue)
        .subscribe(() => {
          this.loadVenues();
          this.toggleForm();
        });
    }
  }

  cancelEdit() {
    this.newVenue = new Venue();
    this.selectedVenue = null;
    this.showForm = false;
  }

  deleteVenue(id: number) {
    if (!confirm('Are you sure?')) return;
    this.venueService.delete('Venue', id).subscribe(() => this.loadVenues());
  }

}
