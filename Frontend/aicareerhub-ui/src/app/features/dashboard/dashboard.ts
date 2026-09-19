import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../layout/navbar/navbar';
import { JobApplicationService } from '../../core/services/job-application.service';
import { CareerProfileService } from '../../core/services/career-profile.service';
import { ResumeService } from '../../core/services/resume.service';
import { JobApplicationStats } from '../../core/models/job-application-stats';
import { CareerProfile } from '../../core/models/career-profile';
import { Resume } from '../../core/models/resume';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  stats: JobApplicationStats | null = null;
  careerProfile: CareerProfile | null = null;
  resumes: Resume[] = [];
  isLoadingStats = true;
  isLoadingProfile = true;
  isLoadingResumes = true;
  statsErrorMessage = '';

  constructor(
    private jobApplicationService: JobApplicationService,
    private careerProfileService: CareerProfileService,
    private resumeService: ResumeService
  ) {}

  ngOnInit(): void {
    this.loadStats();
    this.loadCareerProfile();
    this.loadResumes();
  }

  loadStats(): void {
    this.isLoadingStats = true;
    this.statsErrorMessage = '';
    this.jobApplicationService.getStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        this.isLoadingStats = false;
      },
      error: () => {
        this.statsErrorMessage = 'Unable to load job application statistics.';
        this.isLoadingStats = false;
      }
    });
  }

  loadCareerProfile(): void {
    this.isLoadingProfile = true;
    this.careerProfileService.get().subscribe({
      next: (profile) => {
        this.careerProfile = profile;
        this.isLoadingProfile = false;
      },
      error: () => {
        this.careerProfile = null;
        this.isLoadingProfile = false;
      }
    });
  }

  loadResumes(): void {
    this.isLoadingResumes = true;
    this.resumeService.getAll().subscribe({
      next: (resumes) => {
        this.resumes = resumes;
        this.isLoadingResumes = false;
      },
      error: () => {
        this.resumes = [];
        this.isLoadingResumes = false;
      }
    });
  }
}