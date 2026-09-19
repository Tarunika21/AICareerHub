import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  ImproveResumeSummaryRequest,
  ImproveResumeSummaryResponse,
  ImproveExperienceRequest,
  ImproveExperienceResponse,
  ResumeSuggestionsRequest,
  ResumeSuggestionsResponse,
} from '../models/ai';

@Injectable({
  providedIn: 'root',
})
export class AiService {
  private readonly apiUrl = `${environment.apiUrl}/ai`;

  constructor(private http: HttpClient) {}

  improveResumeSummary(
    request: ImproveResumeSummaryRequest,
  ): Observable<ImproveResumeSummaryResponse> {
    return this.http.post<ImproveResumeSummaryResponse>(`${this.apiUrl}/resume-summary`, request);
  }

  improveExperience(request: ImproveExperienceRequest): Observable<ImproveExperienceResponse> {
    return this.http.post<ImproveExperienceResponse>(`${this.apiUrl}/experience`, request);
  }

  getResumeSuggestions(request: ResumeSuggestionsRequest): Observable<ResumeSuggestionsResponse> {
    return this.http.post<ResumeSuggestionsResponse>(`${this.apiUrl}/resume-suggestions`, request);
  }
}
