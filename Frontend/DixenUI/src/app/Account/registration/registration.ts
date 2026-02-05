import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Authservice } from '../../Services/authservice';

@Component({
  selector: 'app-registration',
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './registration.html',
  styleUrl: './registration.css',
})
// export class Registration {registerForm: FormGroup;
//   successMessage: string = '';
//   errorMessage: string = '';
//   qrCodeImage: string = '';
//   manualKey: string = '';
//   showRegistrationForm = true;  
//   showQRSection = false;


//   constructor(private fb: FormBuilder, private authService: Authservice, private router: Router) {
//     this.registerForm = this.fb.group({
//       email: ['', [Validators.required, Validators.email]],
//       password: ['', Validators.required],
//       fullName: ['', Validators.required],
//       age: ['', Validators.required],
//       gender: ['', Validators.required],
//     });
//   }

// register() {
//   this.authService.register(this.registerForm.value).subscribe({
//     next: (res) => {
//       console.log('Registration response:', res);
      
//       const email = this.registerForm.value.email;
//       const tokenMatch = res.confirmationLink?.match(/token=([^&"]+)/);
//       const token = tokenMatch ? tokenMatch[1] : null;
      
//       console.log('Extracted token:', token);
      
//       if (token) {
//         this.authService.confirmEmail(email, token).subscribe({
//           next: confirmRes => {
//             this.qrCodeImage = confirmRes.qrCodeImage || confirmRes.QrCodeImage;
//             this.manualKey = confirmRes.manualKey;
//             this.successMessage = 'Scan QR to complete setup!';
//             this.showRegistrationForm = false;
//               this.showQRSection = true;
//           },
//           error: () => this.errorMessage = 'Setup failed'
//         });
//       } else {
//         this.errorMessage = 'Token not found in link';
//       }
//     }
//   });
// }
// goBackToForm() {
//     this.showRegistrationForm = true;
//     this.showQRSection = false;
//     this.clearForm();
//   }
//   clearForm() {
//     this.registerForm.reset();
//     this.qrCodeImage = '';
//     this.manualKey = '';
//     this.successMessage = '';
//     this.errorMessage = '';
//   }
// goToLogin() {
//     this.router.navigate(['/login'], { state: { email: this.registerForm.value.email } });
//   }
// }



export class Registration {
  registerForm: FormGroup;
  successMessage = '';
  errorMessage = '';

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
      next: () => {
        this.successMessage = ' Please check your email to confirm registration!';
        this.errorMessage = '';
        this.registerForm.reset(); 
      },
      error: () => this.errorMessage = 'Registration failed'
    });
  }
  resetForm() {
    this.successMessage = '';
    this.errorMessage = '';
    this.registerForm.reset();
  }
}
