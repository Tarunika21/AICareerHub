export interface ImproveResumeSummaryRequest {
  currentSummary: string;
  targetRole: string;
  skills: string;
}

export interface ImproveResumeSummaryResponse {
  improvedSummary: string;
}

export interface ImproveExperienceRequest {
  jobTitle: string;
  company: string;
  currentDescription: string;
  technologies?: string | null;
}

export interface ImproveExperienceResponse {
  improvedDescription: string;
}

export interface ResumeSuggestionsRequest {
  resumeContent: string;
  jobDescription: string;
}

export interface ResumeSuggestionsResponse {
  suggestions: string[];
}