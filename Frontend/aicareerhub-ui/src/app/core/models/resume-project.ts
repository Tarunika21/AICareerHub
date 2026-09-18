export interface ResumeProject {
  id: string;
  name: string;
  description: string;
  technologies: string;
  projectUrl?: string | null;
}

export interface ResumeProjectRequest {
  name: string;
  description: string;
  technologies: string;
  projectUrl?: string | null;
}