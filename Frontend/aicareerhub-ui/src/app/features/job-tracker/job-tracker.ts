import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Navbar } from '../../layout/navbar/navbar';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobApplication } from '../../core/models/job-application';

@Component({
  selector: 'app-job-tracker',
  imports: [CommonModule, Navbar],
  templateUrl: './job-tracker.html',
  styleUrl: './job-tracker.css'
})
export class JobTracker implements OnInit {

  applications: JobApplication[] = [];

  isLoading = true;
  errorMessage = '';

  constructor(
    private jobApplicationService: JobApplicationService
  ) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.jobApplicationService.getAll().subscribe({
      next: (applications) => {
        this.applications = applications;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage =
          'Unable to load job applications.';
        this.isLoading = false;
      }
    });
  }
}