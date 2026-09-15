export interface JobApplicationRequest {
  companyName: string;
  jobTitle: string;
  jobUrl?: string | null;
  location: string;
  status: string;
  appliedDate: string;
  notes?: string | null;
}