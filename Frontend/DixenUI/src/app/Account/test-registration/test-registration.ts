import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Authservice } from '../../Services/authservice';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-test-registration',
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './test-registration.html',
  styleUrl: './test-registration.css',
})
export class TestRegistration {
registerForm: FormGroup;
  successMessage: string = '';
  errorMessage: string = '';
  qrCodeImage: string = '';
  manualKey: string = '';
  showRegistrationForm = true;  
  showQRSection = false;

  
  constructor(private fb: FormBuilder, private authService: Authservice, private router: Router) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      fullName: ['', Validators.required],
      age: ['', Validators.required],
      gender: ['', Validators.required],
    });
  }

register() {
  this.authService.register(this.registerForm.value).subscribe({
    next: (res) => {
      console.log('Registration response:', res);
      
      const email = this.registerForm.value.email;
      const tokenMatch = res.confirmationLink?.match(/token=([^&"]+)/);
      const token = tokenMatch ? tokenMatch[1] : null;
      
      console.log('Extracted token:', token);
      
      if (token) {
        this.authService.confirmEmail(email, token).subscribe({
          next: confirmRes => {
            this.qrCodeImage = confirmRes.qrCodeImage || confirmRes.QrCodeImage;
            this.manualKey = confirmRes.manualKey;
            this.successMessage = 'Scan QR to complete setup!';
            this.showRegistrationForm = false;
              this.showQRSection = true;
          },
          error: () => this.errorMessage = 'Setup failed'
        });
      } else {
        this.errorMessage = 'Token not found in link';
      }
    }
  });
}
goBackToForm() {
    this.showRegistrationForm = true;
    this.showQRSection = false;
    this.clearForm();
  }
  clearForm() {
    this.registerForm.reset();
    this.qrCodeImage = '';
    this.manualKey = '';
    this.successMessage = '';
    this.errorMessage = '';
  }
goToLogin() {
    this.router.navigate(['/login'], { state: { email: this.registerForm.value.email } });
  }
}

