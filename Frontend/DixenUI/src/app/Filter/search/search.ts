import { Component, EventEmitter, Output } from '@angular/core';
import { Evnt } from '../../Models/Evnt';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventResponseDto } from '../../Models/EventResponseDto';
import { EventSearchFilterDto } from '../../Models/EventSearchFilterDto';

@Component({
  selector: 'app-search',
  imports: [CommonModule, FormsModule],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search {
  events: EventResponseDto[] | null=null;
  filter: EventSearchFilterDto = {};
  loading: boolean = false;

  @Output() eventsFound = new EventEmitter<EventResponseDto[]>();
  constructor(private searchService: GenericService<EventResponseDto>) { }

  searchEvents(): void {
    this.loading = true;
    this.searchService.searchEvents(this.filter).subscribe({
      next: (res) => {
        this.events = res;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }
}
