import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { ResumeEducation, ResumeEducationRequest } from '../models/resume-education';

@Injectable({
  providedIn: 'root',
})
export class ResumeEducationService {
  private readonly apiUrl = `${environment.apiUrl}/resumes`;

  constructor(private http: HttpClient) {}

  getAll(resumeId: string): Observable<ResumeEducation[]> {
    return this.http.get<ResumeEducation[]>(`${this.apiUrl}/${resumeId}/educations`);
  }

  create(resumeId: string, request: ResumeEducationRequest): Observable<ResumeEducation> {
    return this.http.post<ResumeEducation>(`${this.apiUrl}/${resumeId}/educations`, request);
  }

  update(
    resumeId: string,
    educationId: string,
    request: ResumeEducationRequest,
  ): Observable<ResumeEducation> {
    return this.http.put<ResumeEducation>(
      `${this.apiUrl}/${resumeId}/educations/${educationId}`,
      request,
    );
  }

  delete(resumeId: string, educationId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${resumeId}/educations/${educationId}`);
  }
}
