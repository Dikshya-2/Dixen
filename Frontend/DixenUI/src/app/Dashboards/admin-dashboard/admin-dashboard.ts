import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { Analysis } from '../../DataAnalysis/analysis/analysis';
import { Analysis2 } from '../../DataAnalysis/analysis2/analysis2';
import { GenericService } from '../../Services/generic-service';
import { Evnt } from '../../Models/Evnt';

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule, Analysis, Analysis2, RouterModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard {
selectedSection: number = 0;
upcomingEvents: Evnt[] = [];
  upcomingCount: number = 0;

  constructor(private router: Router, private eventService: GenericService<Evnt>) {}

  // admin-dashboard.ts  
loadUpcomingEvents() {  // Same as Home
  this.eventService.getAll("Event").subscribe({
    next: (data: Evnt[]) => {
      const now = new Date();
      this.upcomingEvents = data.filter(event => {
        if (!event.startTime) return false;
        const eventDate = new Date(event.startTime);
        return eventDate >= now;  
      });
      this.upcomingCount = this.upcomingEvents.length;
    }
  });
}

ngOnInit() {
  this.loadUpcomingEvents();  // Call it
}

  selectSection(index: number): void {
    this.selectedSection = index;
  }
  
  public route(selector: number): void {
    switch (selector) {
      case 0:
        this.router.navigate([ 'admin/category' ]);
        return;
      case 1:
        this.router.navigate([ 'admin/event' ]);
        return;
      case 2:
        this.router.navigate([ 'admin/hall' ]);
        return;
      case 3:
        this.router.navigate([ 'admin/performer' ]);
        return;
      case 4:
        this.router.navigate([ 'admin/booking' ]);
        return;
        case 5:
        this.router.navigate([ 'admin/ticket' ]);
        return;
         case 6:
        this.router.navigate([ 'admin/venue' ]);
        return;

        case 7: 
      this.router.navigate(['admin/event-submissions']);
      return;
    }
  }
  // Add this property to your AdminDashboard class
menuSections = [
  'Category', 'Event', 'Hall', 'Performer', 
  'Social Share', 'User', 'Admin Submissions'
];

}

