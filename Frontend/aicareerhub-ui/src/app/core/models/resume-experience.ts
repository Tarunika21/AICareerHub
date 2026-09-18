export interface ResumeExperience {
  id: string;
  company: string;
  jobTitle: string;
  startDate: string;
  endDate?: string | null;
  isCurrent: boolean;
  description?: string | null;
}

export interface ResumeExperienceRequest {
  company: string;
  jobTitle: string;
  startDate: string;
  endDate?: string | null;
  isCurrent: boolean;
  description?: string | null;
}