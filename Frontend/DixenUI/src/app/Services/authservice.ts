import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class Authservice {
  // private baseUrl = 'https://localhost:7204/api/auth';
  private baseUrl = `${environment.Apiurl}auth`;
  
  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/login`, { email, password }).pipe(
      tap(response => {
        if (response.requires2FA) {
          // Redirect to 2FA page with email state handled in component
          this.router.navigate(['/verify'], { state: { email } });
        } else if (response.token) {
          localStorage.setItem('jwt', response.token);
          this.router.navigate(['/dashboard']); 
        }
      })
    );
  }

  register(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/register`, data);
  }

  verify2FA(email: string, code: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/verify2fa`, { email, code });
  }

  confirmEmail(email: string, token: string): Observable<any> {
  return this.http.get<any>(`${this.baseUrl}/confirm-email?email=${email}&token=${token}`);
}

  logout() {
    localStorage.removeItem('jwt');
    this.router.navigate(['/login']);
  }

isLoggedIn(): boolean {
  const token = localStorage.getItem('jwt');
  if (!token) return false;

  try {
    const decoded: any = jwtDecode(token);
    console.log('[isLoggedIn] Token expiration:', decoded.exp);
    return decoded.exp * 1000 > Date.now(); // Must still be valid
  } catch (error) {
    console.error('[isLoggedIn] Failed to decode token', error);
    return false;
  }
}

  getUserEmail(): string | null {
    const token = localStorage.getItem('jwt');
    if (!token) return null;

    try {
      const decoded: any = jwtDecode(token);
      return decoded.sub || decoded.email || null;
    } catch (error) {
      console.error('Failed to decode JWT', error);
      return null;
    }
  }
autoSetup2FA(email: string, confirmationLink: string): Observable<any> {
  const url = new URL(confirmationLink);
  const token = url.searchParams.get('token')!;
  return this.confirmEmail(email, token);
}

  
}
