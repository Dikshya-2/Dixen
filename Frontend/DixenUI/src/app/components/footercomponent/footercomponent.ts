import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-footercomponent',
  imports: [CommonModule, TranslateModule, RouterModule],
  templateUrl: './footercomponent.html',
  styleUrl: './footercomponent.css',
})
export class Footercomponent {
email: string = 'dikshyasingh12@gmail.com';
 
  openMaps(address: string): void {
    const encodedAddress = encodeURIComponent(address);
    window.open(`https://www.google.com/maps/dir/?api=1&destination=${encodedAddress}`, '_blank');
  }
  // In your component
serviceRoutes = [
  { key: 'footer.services.0', path: '/create-event' },
  { key: 'footer.services.1', path: '/promote-event' },
  { key: 'footer.services.2', path: '/analytics' },
  { key: 'footer.services.3', path: '/consulting' },
  { key: 'footer.services.4', path: '/help' }
];

}

