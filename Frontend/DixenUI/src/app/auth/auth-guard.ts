import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Authservice } from '../Services/authservice';
import { jwtDecode } from 'jwt-decode';

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree => {
  const authService = inject(Authservice);
  const router = inject(Router);

  const token = localStorage.getItem('jwt');
  if (!token) {
    // No token → redirect to login
    return router.createUrlTree(['/login']);
  }

  try {
    const decoded: any = jwtDecode(token);

    // Check token expiration
    const exp = decoded.exp;
    if (exp && exp * 1000 < Date.now()) {
      return router.createUrlTree(['/login']);
    }

    // Allowed roles for this route
    const allowedRoles: string[] = route.data?.['roles'] || [];

    // Extract roles from token claim
    const roleClaim = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded['role'];
    const userRoles = typeof roleClaim === 'string' 
      ? roleClaim.split(',').map(r => r.trim())  // handle comma-separated string
      : Array.isArray(roleClaim) 
        ? roleClaim 
        : [];

    // Role check
    if (allowedRoles.length && !userRoles.some(role => allowedRoles.includes(role))) {
      return router.createUrlTree(['/unauthorized']);
    }

    // All good → allow access
    return true;

  } catch (err) {
    console.error('[AuthGuard] Failed to decode token:', err);
    return router.createUrlTree(['/login']);
  }
};
