import { CommonModule } from '@angular/common';

import { Component, OnInit } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Navbar } from '../../layout/navbar/navbar';

import { Resume, ResumeRequest } from '../../core/models/resume';

import { ResumeExperience, ResumeExperienceRequest } from '../../core/models/resume-experience';

import { ResumeService } from '../../core/services/resume.service';

import { ResumeExperienceService } from '../../core/services/resume-experience.service';

@Component({
  selector: 'app-resumes',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './resumes.html',
  styleUrl: './resumes.css',
})
export class Resumes implements OnInit {
  // ------------------------------------------------
  // RESUME STATE
  // ------------------------------------------------

  resumes: Resume[] = [];

  isLoading = true;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  editingResumeId: string | null = null;

  // ------------------------------------------------
  // EXPERIENCE STATE
  // ------------------------------------------------

  selectedResume: Resume | null = null;

  experiences: ResumeExperience[] = [];

  isExperienceLoading = false;
  isExperienceSaving = false;

  experienceErrorMessage = '';
  experienceSuccessMessage = '';

  editingExperienceId: string | null = null;

  // ------------------------------------------------
  // FORMS
  // ------------------------------------------------

  resumeForm;

  experienceForm;

  constructor(
    private fb: FormBuilder,
    private resumeService: ResumeService,
    private resumeExperienceService: ResumeExperienceService,
  ) {
    // ----------------------------------------------
    // Resume Form
    // ----------------------------------------------

    this.resumeForm = this.fb.nonNullable.group({
      title: ['', [Validators.required, Validators.maxLength(150)]],

      professionalSummary: ['', [Validators.maxLength(2000)]],

      skills: ['', [Validators.maxLength(1000)]],
    });

    // ----------------------------------------------
    // Experience Form
    // ----------------------------------------------

    this.experienceForm = this.fb.nonNullable.group({
      company: ['', [Validators.required, Validators.maxLength(150)]],

      jobTitle: ['', [Validators.required, Validators.maxLength(150)]],

      startDate: ['', [Validators.required]],

      endDate: [''],

      isCurrent: [false],

      description: ['', [Validators.maxLength(2000)]],
    });
  }

  ngOnInit(): void {
    this.loadResumes();
  }

  // =================================================
  // RESUME
  // =================================================

  loadResumes(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.resumeService.getAll().subscribe({
      next: (resumes) => {
        this.resumes = resumes;
        this.isLoading = false;
      },

      error: (error) => {
        console.error('Failed to load resumes:', error);

        this.errorMessage = 'Unable to load resumes.';

        this.isLoading = false;
      },
    });
  }

  saveResume(): void {
    if (this.resumeForm.invalid) {
      this.resumeForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;

    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.resumeForm.getRawValue();

    const request: ResumeRequest = {
      title: formValue.title.trim(),

      skills: formValue.skills.trim(),

      professionalSummary: formValue.professionalSummary.trim() || null,
    };

    if (this.editingResumeId) {
      this.resumeService.update(this.editingResumeId, request).subscribe({
        next: () => {
          this.isSaving = false;

          this.successMessage = 'Resume updated successfully.';

          this.editingResumeId = null;

          this.resetResumeForm();

          this.loadResumes();
        },

        error: (error) => {
          console.error('Failed to update resume:', error);

          this.isSaving = false;

          this.errorMessage = 'Unable to update resume.';
        },
      });

      return;
    }

    this.resumeService.create(request).subscribe({
      next: () => {
        this.isSaving = false;

        this.successMessage = 'Resume created successfully.';

        this.resetResumeForm();

        this.loadResumes();
      },

      error: (error) => {
        console.error('Failed to create resume:', error);

        this.isSaving = false;

        this.errorMessage = 'Unable to create resume.';
      },
    });
  }

  editResume(resume: Resume): void {
    this.editingResumeId = resume.id;

    this.errorMessage = '';
    this.successMessage = '';

    this.resumeForm.setValue({
      title: resume.title,

      professionalSummary: resume.professionalSummary ?? '',

      skills: resume.skills,
    });

    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  }

  cancelEdit(): void {
    this.editingResumeId = null;

    this.resetResumeForm();

    this.errorMessage = '';
    this.successMessage = '';
  }

  deleteResume(resume: Resume): void {
    const confirmed = window.confirm(`Are you sure you want to delete "${resume.title}"?`);

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.resumeService.delete(resume.id).subscribe({
      next: () => {
        this.successMessage = 'Resume deleted successfully.';

        if (this.editingResumeId === resume.id) {
          this.editingResumeId = null;

          this.resetResumeForm();
        }

        if (this.selectedResume?.id === resume.id) {
          this.closeExperienceManager();
        }

        this.loadResumes();
      },

      error: (error) => {
        console.error('Failed to delete resume:', error);

        this.errorMessage = 'Unable to delete resume.';
      },
    });
  }

  resetResumeForm(): void {
    this.resumeForm.reset({
      title: '',

      professionalSummary: '',

      skills: '',
    });
  }

  // =================================================
  // EXPERIENCE
  // =================================================

  manageExperience(resume: Resume): void {
    this.selectedResume = resume;

    this.editingExperienceId = null;

    this.resetExperienceForm();

    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';

    this.loadExperiences();
  }

  loadExperiences(): void {
    if (!this.selectedResume) {
      return;
    }

    this.isExperienceLoading = true;

    this.experienceErrorMessage = '';

    this.resumeExperienceService.getAll(this.selectedResume.id).subscribe({
      next: (experiences) => {
        this.experiences = experiences;

        this.isExperienceLoading = false;
      },

      error: (error) => {
        console.error('Failed to load experiences:', error);

        this.experienceErrorMessage = 'Unable to load experiences.';

        this.isExperienceLoading = false;
      },
    });
  }

  saveExperience(): void {
    if (!this.selectedResume) {
      return;
    }

    if (this.experienceForm.invalid) {
      this.experienceForm.markAllAsTouched();
      return;
    }

    this.isExperienceSaving = true;

    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';

    const formValue = this.experienceForm.getRawValue();

    /*
     * A current job should not have an end date.
     */
    const endDate = formValue.isCurrent ? null : formValue.endDate || null;

    /*
     * Validate the date relationship on the frontend
     * before sending it to the backend.
     */
    if (endDate && endDate < formValue.startDate) {
      this.isExperienceSaving = false;

      this.experienceErrorMessage = 'End date cannot be earlier than start date.';

      return;
    }

    const request: ResumeExperienceRequest = {
      company: formValue.company.trim(),

      jobTitle: formValue.jobTitle.trim(),

      startDate: formValue.startDate,

      endDate,

      isCurrent: formValue.isCurrent,

      description: formValue.description.trim() || null,
    };

    // ----------------------------------------------
    // UPDATE EXPERIENCE
    // ----------------------------------------------

    if (this.editingExperienceId) {
      this.resumeExperienceService
        .update(this.selectedResume.id, this.editingExperienceId, request)
        .subscribe({
          next: () => {
            this.isExperienceSaving = false;

            this.experienceSuccessMessage = 'Experience updated successfully.';

            this.editingExperienceId = null;

            this.resetExperienceForm();

            this.loadExperiences();
          },

          error: (error) => {
            console.error('Failed to update experience:', error);

            this.isExperienceSaving = false;

            this.experienceErrorMessage = 'Unable to update experience.';
          },
        });

      return;
    }

    // ----------------------------------------------
    // CREATE EXPERIENCE
    // ----------------------------------------------

    this.resumeExperienceService.create(this.selectedResume.id, request).subscribe({
      next: () => {
        this.isExperienceSaving = false;

        this.experienceSuccessMessage = 'Experience added successfully.';

        this.resetExperienceForm();

        this.loadExperiences();
      },

      error: (error) => {
        console.error('Failed to create experience:', error);

        this.isExperienceSaving = false;

        this.experienceErrorMessage = 'Unable to add experience.';
      },
    });
  }

  editExperience(experience: ResumeExperience): void {
    this.editingExperienceId = experience.id;

    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';

    this.experienceForm.setValue({
      company: experience.company,

      jobTitle: experience.jobTitle,

      startDate: experience.startDate,

      endDate: experience.endDate ?? '',

      isCurrent: experience.isCurrent,

      description: experience.description ?? '',
    });
  }

  cancelExperienceEdit(): void {
    this.editingExperienceId = null;

    this.resetExperienceForm();

    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';
  }

  deleteExperience(experience: ResumeExperience): void {
    if (!this.selectedResume) {
      return;
    }

    const confirmed = window.confirm(
      `Are you sure you want to delete your experience at ${experience.company}?`,
    );

    if (!confirmed) {
      return;
    }

    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';

    this.resumeExperienceService.delete(this.selectedResume.id, experience.id).subscribe({
      next: () => {
        this.experienceSuccessMessage = 'Experience deleted successfully.';

        if (this.editingExperienceId === experience.id) {
          this.editingExperienceId = null;

          this.resetExperienceForm();
        }

        this.loadExperiences();
      },

      error: (error) => {
        console.error('Failed to delete experience:', error);

        this.experienceErrorMessage = 'Unable to delete experience.';
      },
    });
  }

  resetExperienceForm(): void {
    this.experienceForm.reset({
      company: '',

      jobTitle: '',

      startDate: '',

      endDate: '',

      isCurrent: false,

      description: '',
    });
  }

  closeExperienceManager(): void {
    this.selectedResume = null;

    this.experiences = [];

    this.editingExperienceId = null;

    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';

    this.resetExperienceForm();
  }
}
