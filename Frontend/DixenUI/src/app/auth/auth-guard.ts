import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Authservice } from '../Services/authservice';
import { jwtDecode } from 'jwt-decode';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(Authservice);
  const router = inject(Router);


  const token = localStorage.getItem('jwt');
  console.log('[AuthGuard] JWT from localStorage:', token);

  if (!token) {
    console.warn('[AuthGuard] No token found. Redirecting to /login');
    router.navigate(['/login']);
    return false;
  }

  try {
    const decoded: any = jwtDecode(token);
    console.log('[AuthGuard] Decoded token:', decoded);

    if (!authService.isLoggedIn()) {
      console.warn('[AuthGuard] Token is expired or invalid. Redirecting to /login');
      router.navigate(['/login']);
      return false;
    }

    const allowedRoles: string[] = route.data?.['roles'] || [];
    const userRolesRaw = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded['role'];
    const userRoles = Array.isArray(userRolesRaw) ? userRolesRaw : [userRolesRaw];

    console.log('[AuthGuard] Allowed roles:', allowedRoles);
    console.log('[AuthGuard] User roles from token:', userRoles);

    if (allowedRoles.length && !userRoles.some(role => allowedRoles.includes(role))) {
      console.warn('[AuthGuard] Role mismatch. Redirecting to /unauthorized');
      router.navigate(['/unauthorized']);
      return false;
    }

    console.log('[AuthGuard] Access granted');
    return true;
  } catch (err) {
    console.error('[AuthGuard] Failed to decode token:', err);
    router.navigate(['/login']);
    return false;
  }
};
