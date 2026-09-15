export interface CareerProfile {
  id: string;
  currentJobTitle: string;
  yearsOfExperience: number;
  skills: string;
  currentLocation: string;
  preferredLocations: string;
  targetRole: string;
  targetSalary?: number | null;
  professionalSummary?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CareerProfileRequest {
  currentJobTitle: string;
  yearsOfExperience: number;
  skills: string;
  currentLocation: string;
  preferredLocations: string;
  targetRole: string;
  targetSalary?: number | null;
  professionalSummary?: string | null;
}