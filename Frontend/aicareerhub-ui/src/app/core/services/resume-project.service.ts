import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { ResumeProject, ResumeProjectRequest } from '../models/resume-project';

@Injectable({
  providedIn: 'root',
})
export class ResumeProjectService {
  private readonly apiUrl = `${environment.apiUrl}/resumes`;

  constructor(private http: HttpClient) {}

  getAll(resumeId: string): Observable<ResumeProject[]> {
    return this.http.get<ResumeProject[]>(`${this.apiUrl}/${resumeId}/projects`);
  }

  create(resumeId: string, request: ResumeProjectRequest): Observable<ResumeProject> {
    return this.http.post<ResumeProject>(`${this.apiUrl}/${resumeId}/projects`, request);
  }

  update(
    resumeId: string,
    projectId: string,
    request: ResumeProjectRequest,
  ): Observable<ResumeProject> {
    return this.http.put<ResumeProject>(
      `${this.apiUrl}/${resumeId}/projects/${projectId}`,
      request,
    );
  }

  delete(resumeId: string, projectId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${resumeId}/projects/${projectId}`);
  }
}
