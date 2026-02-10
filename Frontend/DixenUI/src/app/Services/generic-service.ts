import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SocialShare } from '../Models/SocialShare';
import { EventResponseDto } from '../Models/EventResponseDto';
import { EventSearchFilterDto } from '../Models/EventSearchFilterDto';
import { Category } from '../Models/category';

const httpOptions = {
  headers: new HttpHeaders({
  'Content-Type': 'application/json'
})
}

@Injectable({
  providedIn: 'root',
})
export class GenericService<T, TCreate = T> {
  
  private baseUrl: string =  environment.Apiurl.replace(/\/$/, ''); // Remove trailing slash

  constructor(private httpClient: HttpClient) {}

  private buildUrl(endpoint: string): string {
  return `${this.baseUrl.replace(/\/$/, '')}/${endpoint.replace(/^\//, '')}`;
}

  getAll(endpoint: string): Observable<T[]> {
    return this.httpClient.get<T[]>(this.buildUrl(endpoint));
  }

  getById(endpoint: string, id: number): Observable<T> {
    return this.httpClient.get<T>(`${this.buildUrl(endpoint)}/${id}`, httpOptions);
  }

  // post(endpoint: string, body: T): Observable<T> {
  //   return this.httpClient.post<T>(this.buildUrl(endpoint), body, httpOptions);
  // }
  post(endpoint: string, body: TCreate): Observable<T> {
    return this.httpClient.post<T>(this.buildUrl(endpoint), body, httpOptions);
  }

  put<T>(endpoint: string, id: number, body: any): Observable<T> {
    return this.httpClient.put<T>(`${this.buildUrl(endpoint)}/${id}`, body, httpOptions);
  }

  delete(endpoint: string, id: number): Observable<void> {
    return this.httpClient.delete<void>(`${this.buildUrl(endpoint)}/${id}`, httpOptions);
  }
  // Social Share Methods
  shareEvent(payload: SocialShare): Observable<SocialShare> {
    return this.httpClient.post<SocialShare>(
      this.buildUrl('SocialShare'), 
      payload,
      httpOptions 
    );
  }

  getShareCount(eventId: number): Observable<number> {
    return this.httpClient.get<number>(
      this.buildUrl(`SocialShare/count/${eventId}`), 
      httpOptions
    );
  }
  getByStringId(endpoint: string, id: string): Observable<T> {
  return this.httpClient.get<T>(`${this.buildUrl(endpoint)}/${id}`, httpOptions);
}
getAny<U>(endpoint: string): Observable<U> {
  return this.httpClient.get<U>(this.buildUrl(endpoint), httpOptions);
}

searchEvents(filter: EventSearchFilterDto): Observable<EventResponseDto[]> {
  const url = `${this.baseUrl}/Event/search`;
  return this.httpClient.post<EventResponseDto[]>(url, filter, httpOptions);
}
searchCategories(name: string): Observable<Category[]> {
  return this.httpClient.get<Category[]>(
    `${this.baseUrl}/category/search`,
    { params: { name } } 
  );
}

}
