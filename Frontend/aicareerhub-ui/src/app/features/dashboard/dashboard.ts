import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Navbar } from '../../layout/navbar/navbar';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobApplicationStats } from '../../core/models/job-application-stats';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, Navbar],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  stats: JobApplicationStats | null = null;

  isLoading = true;
  errorMessage = '';

  constructor(
    private jobApplicationService: JobApplicationService
  ) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.jobApplicationService.getStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage =
          'Unable to load dashboard statistics.';
        this.isLoading = false;
      }
    });
  }
}