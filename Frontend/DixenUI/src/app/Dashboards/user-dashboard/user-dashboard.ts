import { Component, OnInit } from '@angular/core';
import { getUserEmailFromToken } from '../../Helper/jwtUtils';
import { UserProfile } from '../../Models/UserProfile';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  imports: [CommonModule, RouterModule],
  templateUrl: './user-dashboard.html',
  styleUrl: './user-dashboard.css',
})
export class UserDashboard implements OnInit {
  user: UserProfile | null = null;
  loading: boolean = true;
  userEmail: string | null = null;
  errorMessage: string | null = null;

  constructor(private userService: GenericService<UserProfile>) {}

  ngOnInit(): void {
    this.userEmail = getUserEmailFromToken();
    if (!this.userEmail) {
      console.error('User email not found in token');
      this.errorMessage = 'User email not found. Please login again.';
      this.loading = false;
      return;
    }

    this.userService.getByStringId('Userprofile/profile/byemail', this.userEmail)
      .subscribe({
        next: (data) => {
          this.user = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error fetching user profile', err);
          this.errorMessage = 'Failed to load user profile.';
          this.loading = false;
        }
      });
  }

}
