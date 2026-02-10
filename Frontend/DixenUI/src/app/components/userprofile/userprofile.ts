import { Component } from '@angular/core';
import { UserProfile } from '../../Models/UserProfile';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Booking } from '../../Models/booking';

@Component({
  selector: 'app-userprofile',
  imports: [CommonModule],
  templateUrl: './userprofile.html',
  styleUrl: './userprofile.css',
})
export class Userprofile {
  user: UserProfile | null = null;
  userBookings: Booking[] = [];
  isLoading = true;
  email: string = '';

  roles: Array<'User' | 'Host' | 'Admin'> = ['User', 'Host', 'Admin'];

  rolesInfo: Record<'User' | 'Host' | 'Admin', { description: string }> = {
    User: {
      description: `General platform user who can browse and attend events.
        Register and manage profile
        Browse and search events
        Register for events
        Propose events
        Share events on social media
        Manage 2FA and security settings`,
    },
    Host: {
      description: `Users responsible for creating and managing events.
        All User permissions
        Create, edit, categorize, and delete events
        Assign performers and venues
        View analytics dashboard
        Submit event proposals for admin approval`,
    },
    Admin: {
      description: `Platform administrators with full system control.
        Approve/reject event proposals
        Manage user roles and permissions
        Access and manage all events, users, and system settings
        Monitor security and logs`,
    },
  };

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    this.email = this.route.snapshot.paramMap.get('email') || '';

    if (!this.email) {
      console.error('User email not found in route');
      this.isLoading = false;
      return;
    }

    this.loadUserProfile();
    this.loadUserBookings();
  }

  loadUserProfile(): void {
    this.http
      .get<UserProfile>(
        `https://localhost:7204/api/Userprofile/profile/byemail/${encodeURIComponent(this.email)}`,
      )
      .subscribe({
        next: (data) => {
          this.user = data;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Failed to load user profile', err);
          this.isLoading = false;
        },
      });
  }

  loadUserBookings(): void {
    this.http
      .get<Booking[]>('https://localhost:7204/api/Booking/all')
      .subscribe({
        next: (bookings) => (this.userBookings = bookings),
        error: (err) => console.error('Failed to load bookings', err),
      });
  }

  getRoleDescription(role: string): string[] {
    const key = role as keyof typeof this.rolesInfo;
    return this.rolesInfo[key].description
      .split('\n')
      .filter((line) => line.trim() !== '');
  }
}
