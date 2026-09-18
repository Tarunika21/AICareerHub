import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  ResumeExperience,
  ResumeExperienceRequest
} from '../models/resume-experience';

@Injectable({
  providedIn: 'root'
})
export class ResumeExperienceService {

  private readonly apiUrl =
    `${environment.apiUrl}/resumes`;

  constructor(
    private http: HttpClient
  ) {}

  getAll(
    resumeId: string
  ): Observable<ResumeExperience[]> {

    return this.http.get<ResumeExperience[]>(
      `${this.apiUrl}/${resumeId}/experiences`
    );
  }

  create(
    resumeId: string,
    request: ResumeExperienceRequest
  ): Observable<ResumeExperience> {

    return this.http.post<ResumeExperience>(
      `${this.apiUrl}/${resumeId}/experiences`,
      request
    );
  }

  update(
    resumeId: string,
    experienceId: string,
    request: ResumeExperienceRequest
  ): Observable<ResumeExperience> {

    return this.http.put<ResumeExperience>(
      `${this.apiUrl}/${resumeId}/experiences/${experienceId}`,
      request
    );
  }

  delete(
    resumeId: string,
    experienceId: string
  ): Observable<void> {

    return this.http.delete<void>(
      `${this.apiUrl}/${resumeId}/experiences/${experienceId}`
    );
  }
}