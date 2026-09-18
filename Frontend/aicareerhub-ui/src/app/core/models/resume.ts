export interface Resume {
  id: string;
  title: string;
  professionalSummary?: string | null;
  skills: string;
  createdAt: string;
  updatedAt: string;
}

export interface ResumeRequest {
  title: string;
  professionalSummary?: string | null;
  skills: string;
}