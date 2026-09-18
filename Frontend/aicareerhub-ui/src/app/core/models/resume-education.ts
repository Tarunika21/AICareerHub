export interface ResumeEducation {
  id: string;
  institution: string;
  degree: string;
  fieldOfStudy?: string | null;
  startYear: number;
  endYear?: number | null;
}

export interface ResumeEducationRequest {
  institution: string;
  degree: string;
  fieldOfStudy?: string | null;
  startYear: number;
  endYear?: number | null;
}