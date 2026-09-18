import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../layout/navbar/navbar';
import { Resume, ResumeRequest } from '../../core/models/resume';
import { ResumeExperience, ResumeExperienceRequest } from '../../core/models/resume-experience';
import { ResumeService } from '../../core/services/resume.service';
import { ResumeExperienceService } from '../../core/services/resume-experience.service';

@Component({
  selector: 'app-resume-builder',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './resume-builder.html',
  styleUrl: './resume-builder.css',
})
export class ResumeBuilder implements OnInit {
  // ==============================
  // RESUME
  // ==============================

  resumeId: string | null = null;

  resume: Resume | null = null;

  isLoading = false;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  // ==============================
  // EXPERIENCE
  // ==============================

  experiences: ResumeExperience[] = [];

  isExperienceLoading = false;
  isExperienceSaving = false;

  experienceErrorMessage = '';
  experienceSuccessMessage = '';

  editingExperienceId: string | null = null;

  // ==============================
  // RESUME FORM
  // ==============================

  resumeForm;

  // ==============================
  // EXPERIENCE FORM
  // ==============================

  experienceForm;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private resumeService: ResumeService,
    private resumeExperienceService: ResumeExperienceService,
  ) {
    this.resumeForm = this.fb.nonNullable.group({
      title: ['', [Validators.required, Validators.maxLength(150)]],
      skills: ['', [Validators.maxLength(1000)]],
      professionalSummary: ['', [Validators.maxLength(2000)]],
    });

    this.experienceForm = this.fb.nonNullable.group({
      company: ['', [Validators.required, Validators.maxLength(150)]],
      jobTitle: ['', [Validators.required, Validators.maxLength(150)]],
      startDate: ['', [Validators.required]],
      endDate: [''],
      isCurrent: [false],
      description: ['', [Validators.maxLength(2000)]],
    });
  }

  // ==============================
  // INITIALIZATION
  // ==============================

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.resumeId = id;
      this.loadResume();
      this.loadExperiences();
    }
  }

  // ==============================
  // RESUME
  // ==============================

  loadResume(): void {
    if (!this.resumeId) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.resumeService.getById(this.resumeId).subscribe({
      next: (resume) => {
        this.resume = resume;

        this.resumeForm.patchValue({
          title: resume.title,
          skills: resume.skills,
          professionalSummary: resume.professionalSummary ?? '',
        });

        this.isLoading = false;
      },

      error: (error) => {
        console.error('Failed to load resume:', error);
        this.errorMessage = 'Unable to load resume.';
        this.isLoading = false;
      },
    });
  }

  saveBasicDetails(): void {
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

    // ============================
    // UPDATE EXISTING RESUME
    // ============================

    if (this.resumeId) {
      this.resumeService.update(this.resumeId, request).subscribe({
        next: (resume) => {
          this.resume = resume;
          this.isSaving = false;
          this.successMessage = 'Resume details saved successfully.';
        },

        error: (error) => {
          console.error('Failed to update resume:', error);
          this.isSaving = false;
          this.errorMessage = 'Unable to save resume.';
        },
      });

      return;
    }

    // ============================
    // CREATE NEW RESUME
    // ============================

    this.resumeService.create(request).subscribe({
      next: (resume) => {
        this.resume = resume;
        this.resumeId = resume.id;
        this.isSaving = false;
        this.successMessage = 'Basic details saved. Continue building your resume.';
        this.router.navigate(['/resumes', resume.id, 'edit'], {
          replaceUrl: true,
        });
      },

      error: (error) => {
        console.error('Failed to create resume:', error);
        this.isSaving = false;
        this.errorMessage = 'Unable to create resume.';
      },
    });
  }

  // ==============================
  // EXPERIENCE
  // ==============================

  loadExperiences(): void {
    if (!this.resumeId) {
      return;
    }

    this.isExperienceLoading = true;
    this.experienceErrorMessage = '';

    this.resumeExperienceService.getAll(this.resumeId).subscribe({
      next: (experiences) => {
        this.experiences = experiences;
        this.isExperienceLoading = false;
      },

      error: (error) => {
        console.error('Failed to load experiences:', error);
        this.experienceErrorMessage = 'Unable to load work experience.';
        this.isExperienceLoading = false;
      },
    });
  }

  saveExperience(): void {
    if (!this.resumeId) {
      this.experienceErrorMessage = 'Save Basic Details first.';

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
    const endDate = formValue.isCurrent ? null : formValue.endDate || null;

    // ============================
    // DATE VALIDATION
    // ============================

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

    // ============================
    // UPDATE EXPERIENCE
    // ============================

    if (this.editingExperienceId) {
      this.resumeExperienceService
        .update(this.resumeId, this.editingExperienceId, request)
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

    // ============================
    // CREATE EXPERIENCE
    // ============================

    this.resumeExperienceService.create(this.resumeId, request).subscribe({
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
    this.experienceForm.patchValue({
      company: experience.company,
      jobTitle: experience.jobTitle,
      startDate: experience.startDate,
      endDate: experience.endDate ?? '',
      isCurrent: experience.isCurrent,
      description: experience.description ?? '',
    });

    setTimeout(() => {
      document.getElementById('experience-form')?.scrollIntoView({
        behavior: 'smooth',
      });
    }, 0);
  }

  cancelExperienceEdit(): void {
    this.editingExperienceId = null;
    this.resetExperienceForm();
    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';
  }

  deleteExperience(experience: ResumeExperience): void {
    if (!this.resumeId) {
      return;
    }

    const confirmed = confirm(`Delete your experience at ${experience.company}?`);

    if (!confirmed) {
      return;
    }

    this.experienceErrorMessage = '';
    this.experienceSuccessMessage = '';
    this.resumeExperienceService.delete(this.resumeId, experience.id).subscribe({
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
}
