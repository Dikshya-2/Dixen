import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Analysis } from '../../DataAnalysis/analysis/analysis';
import { Analysis2 } from '../../DataAnalysis/analysis2/analysis2';
import { UserProfile } from '../../Models/UserProfile';
import { GenericService } from '../../Services/generic-service';
import { getUserEmailFromToken } from '../../Helper/jwtUtils';

@Component({
  selector: 'app-host-dashboard',
  imports: [RouterModule,Analysis,Analysis2],
  templateUrl: './host-dashboard.html',
  styleUrl: './host-dashboard.css',
})
export class HostDashboard {
    organizerName = 'ABC';
    host: UserProfile | null = null;
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
            this.host = data;
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
