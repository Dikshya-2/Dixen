import { Component } from '@angular/core';
import { Evnt } from '../../Models/Evnt';
import { GenericService } from '../../Services/generic-service';
import { Category } from '../../Models/category';
import { Router, RouterModule } from '@angular/router';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { CategoryList } from '../../Detail/category-list/category-list';
import { Search } from '../../Filter/search/search';
import { EventResponseDto } from '../../Models/EventResponseDto';
import { RecommendedEvent } from '../recommended-event/recommended-event';

@Component({
  selector: 'app-home',
  imports: [CommonModule,RouterModule, CategoryList,Search, RecommendedEvent],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  fallbackImage: string = '/assets/default.jpg';
  events: Evnt[] = [];
   event: EventResponseDto[] = [];

  constructor(
    private eventService: GenericService<Evnt>,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }
  loadEvents(): void {
  this.eventService.getAll("Event").subscribe({
    next: (data: Evnt[]) => {
      const now = new Date();
      this.events = data.filter(event => {
        if (!event.startTime) return false;
        const eventDate = new Date(event.startTime);
        return eventDate >= now;  
      });
      console.log('Upcoming events:', this.events.length);
    },
    error: (err) => console.error('Failed to load events', err)
  });
}
}
 

