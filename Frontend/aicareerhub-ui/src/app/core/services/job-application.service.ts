import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { JobApplication } from '../models/job-application';
import { JobApplicationRequest } from '../models/job-application-request';
import { JobApplicationStats } from '../models/job-application-stats';

@Injectable({
  providedIn: 'root'
})
export class JobApplicationService {
  private readonly apiUrl =
    `${environment.apiUrl}/job-applications`;

  constructor(private http: HttpClient) {}

  getAll(
    status?: string,
    search?: string
  ): Observable<JobApplication[]> {

    let params = new HttpParams();

    if (status) {
      params = params.set('status', status);
    }

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<JobApplication[]>(
      this.apiUrl,
      { params }
    );
  }

  getById(id: string): Observable<JobApplication> {
    return this.http.get<JobApplication>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: JobApplicationRequest
  ): Observable<JobApplication> {

    return this.http.post<JobApplication>(
      this.apiUrl,
      request
    );
  }

  update(
    id: string,
    request: JobApplicationRequest
  ): Observable<JobApplication> {

    return this.http.put<JobApplication>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }

  getStats(): Observable<JobApplicationStats> {
    return this.http.get<JobApplicationStats>(
      `${this.apiUrl}/stats`
    );
  }
}