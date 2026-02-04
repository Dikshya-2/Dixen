import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-conform-email',
  imports: [CommonModule, TranslateModule],
  templateUrl: './conform-email.html',
  styleUrl: './conform-email.css',
})
export class ConformEmail {
  message: string = '';
  qrCodeImage: string = '';
  manualKey: string = '';
  error: string = '';
  email: string = '';

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParamMap.get('email') || '';
    const token = this.route.snapshot.queryParamMap.get('token');
    console.log('EMAIL:', this.email);
    console.log('TOKEN:', token);

    if (this.email && token) {
      this.http
        .get<any>(
          `https://localhost:7204/api/auth/confirm-email?email=${this.email}&token=${token}`,
        )
        .subscribe({
          next: (res) => {
            this.message = res.message || res.Message;
            this.qrCodeImage = res.QrCodeImage || res.qrCodeImage;
            this.manualKey = res.ManualKey || res.manualKey;

            console.log('QR LENGTH:', this.qrCodeImage?.length);
            console.log('MANUAL KEY:', this.manualKey);
          },
          error: (err) => {
            this.error = 'confirmEmail.failedConfirm';
            this.router.navigate(['/login']);
          },
        });
    } else {
      this.error = 'confirmEmail.tokenMissing';
      this.router.navigate(['/login']);
    }
  }

  goToLogin() {
    this.router.navigate(['/login'], { state: { email: this.email } });
  }
}
