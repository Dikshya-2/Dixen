import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Authservice } from '../../Services/authservice';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginForm: FormGroup;
  error: string = '';
  showPassword: boolean = false;


  constructor(private fb: FormBuilder, private authService: Authservice) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  login() {
    const { email, password } = this.loginForm.value;
    this.authService.login(email, password).subscribe({
      error: err => this.error = err.error || 'Login failed'
    });
  }
}

