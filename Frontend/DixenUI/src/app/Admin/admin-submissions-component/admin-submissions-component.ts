import { Component } from '@angular/core';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { EventSubmission } from '../../Models/EventSubmissionDto';

@Component({
  selector: 'app-admin-submissions-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-submissions-component.html',
  styleUrl: './admin-submissions-component.css',
})
export class AdminSubmissionsComponent {
submissions: EventSubmission[] = [];
  
  constructor(private service: GenericService<EventSubmission>) {}

  ngOnInit(): void {
    this.loadSubmissions();
  }

  loadSubmissions(): void {
    this.service.post('EventSubmission/event-submissions/list', {}).subscribe({
      next: (data) => {
        this.submissions = Array.isArray(data) ? data : [];
        console.log('Loaded:', this.submissions);
      },
      error: (err) => console.error('Error:', err)
    });
  }

 approveSubmission(id?: number): void {
  if (!id) return;
  if (!confirm('Approve?')) return;

  this.service.post(`EventSubmission/event-submissions/${id}/approve`, {}).subscribe({
    next: () => { 
      alert('Approved!');
      this.loadSubmissions();
    },
    error: (err) => alert('Error: ' + (err.error || 'Unknown'))
  });
}

rejectSubmission(id?: number): void {
  if (!id) return;
  if (!confirm('Reject?')) return;

  this.service.post(`EventSubmission/event-submissions/${id}/reject`, {}).subscribe({
    next: () => { 
      alert('Rejected!');
      this.loadSubmissions();
    },
    error: (err) => alert('Error: ' + (err.error || 'Unknown'))
  });
}

}
