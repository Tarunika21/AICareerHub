export interface JobApplication {
  id: string;
  companyName: string;
  jobTitle: string;
  jobUrl?: string | null;
  location: string;
  status: string;
  appliedDate: string;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}