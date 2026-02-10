import { Component } from '@angular/core';
import { Evnt } from '../../Models/Evnt';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-recommended-event',
  imports: [CommonModule, RouterModule],
  templateUrl: './recommended-event.html',
  styleUrl: './recommended-event.css',
})
export class RecommendedEvent {
  events: Evnt[] = [];
  recommended: Evnt[] = [];
  shareCounts: { [eventId: number]: number } = {};

  constructor(
    private eventService: GenericService<Evnt>,
    private shareService: GenericService<any>
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventService.getAll('Event').subscribe({
      next: (events) => {
        this.events = events;
        this.fetchShareCounts();
      },
      error: (err) => console.error('Failed to fetch events', err),
    });
  }

  trackByEventId(index: number, event: Evnt): number {
    return event.id;
  }

  fetchShareCounts(): void {
    let completed = 0;

    this.events.forEach((event) => {
      this.shareService.getShareCount(event.id).subscribe({
        next: (count) => {
          this.shareCounts[event.id] = count;
          completed++;

          if (completed === this.events.length) {
            this.recommended = this.events
              .filter((e) => this.shareCounts[e.id] > 3)
              .sort((a, b) => this.shareCounts[b.id] - this.shareCounts[a.id])
              .slice(0, 5); // Top 5 most shared events
          }
        },
        error: (err) => {
          console.error(`Failed to get share count for event ${event.id}`, err);
          completed++;
        },
      });
    });
  }

}
