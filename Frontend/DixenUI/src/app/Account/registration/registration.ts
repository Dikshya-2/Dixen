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
