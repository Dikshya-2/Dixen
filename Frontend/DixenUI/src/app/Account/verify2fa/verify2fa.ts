import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Authservice } from '../../Services/authservice';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-verify2fa',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './verify2fa.html',
  styleUrl: './verify2fa.css',
})
export class Verify2fa {
  form: FormGroup;
  email: string;
  error: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: Authservice,
    private router: Router
  ) {
    const nav = this.router.currentNavigation();
    this.email = nav?.extras?.state?.['email'] || '';

    if (!this.email) {
      this.router.navigate(['/login']);
    }

    this.form = this.fb.group({
      code: ['', Validators.required]
    });
  }
verify() {
  if (this.form.invalid) {
    this.error = 'Please enter the verification code.';
    return;
  }

  this.authService.verify2FA(this.email, this.form.value.code).subscribe({
    next: res => {
      if (res.token) {
        localStorage.setItem('jwt', res.token); // Ensure token is stored

        const decoded: any = jwtDecode(res.token);
        const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded['role'];

        // Delay route navigation to ensure AuthGuard reads updated localStorage
        setTimeout(() => {
          if (role === 'Admin') {
            this.router.navigate(['/admin-dashboard']);

          } else if (role === 'Host') {
            this.router.navigate(['/host-dashboard']);
          } else if (role === 'User') {
            this.router.navigate(['/user-dashboard']);
          } else {
            this.error = 'Unknown role';
          }
        }, 0);

      } else {
        this.error = 'Token was not returned from server';
      }
    },
    error: err => {
      this.error = err.error || '2FA verification failed';
    }
  });

}
}
