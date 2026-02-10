import { Component } from '@angular/core';
import { EventSubmissionDto } from '../../Models/EventSubmissionDto';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-submit-event-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './submit-event-component.html',
  styleUrl: './submit-event-component.css',
})
export class SubmitEventComponent {
organizerId = 1; 
  loading = false;
  message = '';
  submittedEvents: EventSubmissionDto[] = [];

  submission: EventSubmissionDto = {
    eventId: 0,
    title: '',
    description: '',
    startTime: ''
  };

  constructor(
    private service: GenericService<any, EventSubmissionDto>
  ) {}

  submit() {
    this.loading = true;

    this.service
      .post(`Organizer/${this.organizerId}/submit-event`, this.submission)
      .subscribe({
        next: (res: any) => {
          this.message = res.message;
          this.loading = false;
        },
        error: err => {
          this.message = err.error;
          this.loading = false;
        }
      });
  }
  
}
