import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CareerProfile,
  CareerProfileRequest
} from '../models/career-profile';

@Injectable({
  providedIn: 'root'
})
export class CareerProfileService {

  private readonly apiUrl =
    `${environment.apiUrl}/career-profile`;

  constructor(
    private http: HttpClient
  ) {}

  get(): Observable<CareerProfile> {
    return this.http.get<CareerProfile>(
      this.apiUrl
    );
  }

  create(
    request: CareerProfileRequest
  ): Observable<CareerProfile> {

    return this.http.post<CareerProfile>(
      this.apiUrl,
      request
    );
  }

  update(
    request: CareerProfileRequest
  ): Observable<CareerProfile> {

    return this.http.put<CareerProfile>(
      this.apiUrl,
      request
    );
  }
}