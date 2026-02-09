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
  userId: string = '';
  isLoading = true;
  userBookings: Booking[] = [];

rolesInfo: Record<'User' | 'Host' | 'Admin', { description: string }> = {
  User: {
    description: `General platform user who can browse and attend events.
  Register and manage profile
  Browse and search events
  Register for events
  Propose events
  Share events on social media
  Manage 2FA and security settings`
  },
  Host: {
    description: `Users responsible for creating and managing events.
  All User permissions
  Create, edit, categorize, and delete events
  Assign performers and venues
  View analytics dashboard
  Submit event proposals for admin approval`
  },
  Admin: {
    description: `Platform administrators with full system control.
  All Organizer permissions
  Approve/reject event proposals
  Manage user roles and permissions
  Access and manage all events, users, and system settings
  Monitor security and logs`
  }
};
  roles: Array<'User' | 'Host' | 'Admin'> = ['User', 'Host', 'Admin'];

  constructor(private http: HttpClient, private route: ActivatedRoute) {}
ngOnInit(): void {
  this.userId = this.route.snapshot.paramMap.get('email') || '';
  if (this.userId) {
    this.http.get<UserProfile>(`https://localhost:7204/api/Userprofile/profile/byemail/${encodeURIComponent(this.userId)}`)
      .subscribe({
        next: (data) => {
          this.user = data;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Failed to load user profile', err);
          this.isLoading = false;
        }
      });
  } else {
    console.error('User email not found in route');
    this.isLoading = false;
  }

}
fetchUserBookings(): void {
    this.http.get<Booking[]>('https://localhost:7204/api/Booking/all')
      .subscribe({
        next: (bookings) => {
          this.userBookings = bookings;
        },
        error: (err) => {
          console.error('Failed to fetch user bookings', err);
        }
      });
  }
getRoleDescription(role: string): string[] {
  const key = role as keyof typeof this.rolesInfo;
  return (this.rolesInfo[key]?.description.split('\n') || []).filter(line => line.trim() !== '');
}

}
