import { jwtDecode } from 'jwt-decode';

export function getUserEmailFromToken(): string | null {
  const token = localStorage.getItem('jwt');
  if (!token) return null;

  try {
    const decoded: any = jwtDecode(token);
    console.log('FULL DECODED TOKEN:', decoded);
    return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? null;
    
  } catch (error) {
    console.error('Failed to decode JWT', error);
    return null;
  }
}

export function getUserIdFromToken(): string | null {
  const token = localStorage.getItem('jwt');
  if (!token) return null;

  try {
    const decoded: any = jwtDecode(token);

    return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? null;

  } catch (error) {
    console.error('Failed to decode JWT', error);
    return null;
  }
}




