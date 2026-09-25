import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Navbar } from '../../layout/navbar/navbar';

import { Resume } from '../../core/models/resume';
import { ResumeExperience } from '../../core/models/resume-experience';
import { ResumeEducation } from '../../core/models/resume-education';
import { ResumeProject } from '../../core/models/resume-project';

import { ResumeService } from '../../core/services/resume.service';
import { ResumeExperienceService } from '../../core/services/resume-experience.service';
import { ResumeEducationService } from '../../core/services/resume-education.service';
import { ResumeProjectService } from '../../core/services/resume-project.service';

@Component({
  selector: 'app-resume-preview',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './resume-preview.html',
  styleUrl: './resume-preview.css',
})
export class ResumePreview implements OnInit {
  resumeId: string | null = null;

  resume: Resume | null = null;

  experiences: ResumeExperience[] = [];
  educations: ResumeEducation[] = [];
  projects: ResumeProject[] = [];

  isLoading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private resumeService: ResumeService,
    private resumeExperienceService: ResumeExperienceService,
    private resumeEducationService: ResumeEducationService,
    private resumeProjectService: ResumeProjectService,
  ) {}

  ngOnInit(): void {
    this.resumeId = this.route.snapshot.paramMap.get('id');

    if (!this.resumeId) {
      this.errorMessage = 'Resume ID is missing.';
      this.isLoading = false;
      return;
    }

    this.loadResumePreview();
  }

  loadResumePreview(): void {
    if (!this.resumeId) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    let completedRequests = 0;
    const totalRequests = 4;

    const markComplete = (): void => {
      completedRequests++;

      if (completedRequests === totalRequests) {
        this.isLoading = false;
      }
    };

    this.resumeService.getById(this.resumeId).subscribe({
      next: (resume) => {
        this.resume = resume;
        markComplete();
      },

      error: (error) => {
        console.error('Failed to load resume:', error);

        this.errorMessage = 'Unable to load resume preview.';
        markComplete();
      },
    });

    this.resumeExperienceService.getAll(this.resumeId).subscribe({
      next: (experiences) => {
        this.experiences = experiences;
        markComplete();
      },

      error: (error) => {
        console.error('Failed to load experiences:', error);
        markComplete();
      },
    });

    this.resumeEducationService.getAll(this.resumeId).subscribe({
      next: (educations) => {
        this.educations = educations;
        markComplete();
      },

      error: (error) => {
        console.error('Failed to load education:', error);
        markComplete();
      },
    });

    this.resumeProjectService.getAll(this.resumeId).subscribe({
      next: (projects) => {
        this.projects = projects;
        markComplete();
      },

      error: (error) => {
        console.error('Failed to load projects:', error);
        markComplete();
      },
    });
  }

  printResume(): void {
    window.print();
  }
}