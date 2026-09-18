import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  Resume,
  ResumeRequest
} from '../models/resume';

@Injectable({
  providedIn: 'root'
})
export class ResumeService {

  private readonly apiUrl =
    `${environment.apiUrl}/resumes`;

  constructor(
    private http: HttpClient
  ) {}

  getAll(): Observable<Resume[]> {

    return this.http.get<Resume[]>(
      this.apiUrl
    );
  }

  getById(id: string): Observable<Resume> {

    return this.http.get<Resume>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: ResumeRequest
  ): Observable<Resume> {

    return this.http.post<Resume>(
      this.apiUrl,
      request
    );
  }

  update(
    id: string,
    request: ResumeRequest
  ): Observable<Resume> {

    return this.http.put<Resume>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  delete(id: string): Observable<void> {

    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}