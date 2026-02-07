import { Component } from '@angular/core';
import { Evnt } from '../../Models/Evnt';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { Category } from '../../Models/category';
import { SocialShare } from '../../Models/SocialShare';
import { getUserEmailFromToken, getUserIdFromToken } from '../../Helper/jwtUtils';
import { EventReview } from '../../Models/EventReview';

@Component({
  selector: 'app-category-detail',
  imports: [CommonModule, RouterModule],
  templateUrl: './category-detail.html',
  styleUrl: './category-detail.css',
})
export class CategoryDetail {
  events: Evnt[] = [];
  // categoryName: string = '';
  category!: Category;
  fallbackImage: string = '/assets/default.jpg';
  shareCounts:{[eventId:number]:number}={}
shareVisible:{[eventId:number]:boolean}={}
// store the user rating per event
ratings: { [eventId: number]: number } = {};


  constructor(
    private route: ActivatedRoute,
    private eventService: GenericService<Evnt>,
    public router: Router,
    private categoryService: GenericService<Category>,
    private socialShareService: GenericService<SocialShare>,
    private reviewService: GenericService<EventReview>

  ) {}
  ngOnInit(): void {
    const categoryId = Number(this.route.snapshot.paramMap.get('categoryId'));

    this.categoryService.getById('Category', categoryId).subscribe({
      next: (data) => {
        this.category = data;
        this.events = data.events ?? [];
        this.events.forEach((e) => this.getShareCount(e.id));
      },
      error: (err) => console.error('Failed to load category', err),
    });
  }

  goToEvent(id: number) {
    this.router.navigate(['/event', id]);
  }

  trackByEventId(index: number, event: Evnt): number {
    return event.id;
  }

  share(platform: string, event: Evnt): void {
  const userEmail = getUserEmailFromToken(); // may be null
  const pageUrl = encodeURIComponent(
    `${window.location.origin}/event/${event.id}`
  );
  const text = encodeURIComponent(`Check out this event: ${event.title}`);

  const shareUrlMap: Record<string, string> = {
    facebook: `https://www.facebook.com/sharer/sharer.php?u=${pageUrl}`,
    twitter: `https://twitter.com/intent/tweet?text=${text}&url=${pageUrl}`,
    whatsapp: `https://wa.me/?text=${text}%20${pageUrl}`,
  };

  // Open share window FIRST (UX-friendly)
  window.open(shareUrlMap[platform], '_blank');

  // Save share (userEmail optional)
  const payload: SocialShare = {
    eventId: event.id,
    platform,
    userEmail: userEmail ?? undefined,
  };

  this.socialShareService.shareEvent(payload).subscribe({
    next: () => {
      // Optimistic UI update
      this.shareCounts[event.id] = (this.shareCounts[event.id] || 0) + 1;
    },
    error: (err) => console.error('Failed to save share', err),
  });
}

  getShareCount(eventId: number): void {
    this.socialShareService.getShareCount(eventId).subscribe({
      next: (count) => (this.shareCounts[eventId] = count),
      error: (err) => console.error(err),
    });
  }
  toggleShare(eventId: number): void {
    this.shareVisible[eventId] = !this.shareVisible[eventId];
  }
 // In your CategoryDetail component
rateEvent(eventId: number, rating: number) {
  const userId = getUserIdFromToken();
  if (!userId) return alert('Login to rate');

  // Save user rating locally
  this.ratings[eventId] = rating;

  // Create payload
  const payload: EventReview= {
    eventId: eventId,
    userId: userId,
    rating: rating,
    comment: ''
  };

  // Send to backend
  this.reviewService.post('EventReview', payload as any)
    .subscribe(() => {
      this.loadAverageRating(eventId);
    });
}

averageRatings: { [eventId: number]: number } = {}; // declare at top

loadAverageRating(eventId: number) {
  this.reviewService.getAny<number>(`EventReview/average/${eventId}`)
    .subscribe(avg => {
      this.averageRatings[eventId] = avg;
    });
}


}
 