import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../layout/navbar/navbar';
import { Resume, ResumeRequest } from '../../core/models/resume';
import { ResumeExperience, ResumeExperienceRequest } from '../../core/models/resume-experience';
import { ResumeService } from '../../core/services/resume.service';
import { ResumeExperienceService } from '../../core/services/resume-experience.service';
import { ResumeEducation, ResumeEducationRequest } from '../../core/models/resume-education';
import { ResumeEducationService } from '../../core/services/resume-education.service';

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
  // EDUCATION
  // ==============================

  educations: ResumeEducation[] = [];

  isEducationLoading = false;
  isEducationSaving = false;

  educationErrorMessage = '';
  educationSuccessMessage = '';

  editingEducationId: string | null = null;
  // ==============================
  // RESUME FORM
  // ==============================

  resumeForm;

  // ==============================
  // EXPERIENCE FORM
  // ==============================

  experienceForm;
  educationForm;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private resumeService: ResumeService,
    private resumeExperienceService: ResumeExperienceService,
    private resumeEducationService: ResumeEducationService,
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
    this.educationForm = this.fb.nonNullable.group({
      institution: ['', [Validators.required, Validators.maxLength(200)]],

      degree: ['', [Validators.required, Validators.maxLength(150)]],

      fieldOfStudy: ['', [Validators.maxLength(150)]],

      startYear: [
        new Date().getFullYear(),
        [Validators.required, Validators.min(1950), Validators.max(2100)],
      ],

      endYear: [null as number | null, [Validators.min(1950), Validators.max(2100)]],
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
      this.loadEducations();
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

  loadEducations(): void {
    if (!this.resumeId) {
      return;
    }

    this.isEducationLoading = true;
    this.educationErrorMessage = '';

    this.resumeEducationService.getAll(this.resumeId).subscribe({
      next: (educations) => {
        this.educations = educations;

        this.isEducationLoading = false;
      },

      error: (error) => {
        console.error('Failed to load education:', error);

        this.educationErrorMessage = 'Unable to load education.';

        this.isEducationLoading = false;
      },
    });
  }

  saveEducation(): void {
    if (!this.resumeId) {
      this.educationErrorMessage = 'Save Basic Details first.';

      return;
    }

    if (this.educationForm.invalid) {
      this.educationForm.markAllAsTouched();

      return;
    }

    this.isEducationSaving = true;

    this.educationErrorMessage = '';
    this.educationSuccessMessage = '';

    const formValue = this.educationForm.getRawValue();

    const startYear = Number(formValue.startYear);

    const endYear =
      formValue.endYear === null || formValue.endYear === undefined
        ? null
        : Number(formValue.endYear);

    // End year cannot precede start year.

    if (endYear !== null && endYear < startYear) {
      this.isEducationSaving = false;

      this.educationErrorMessage = 'End year cannot be earlier than start year.';

      return;
    }

    const request: ResumeEducationRequest = {
      institution: formValue.institution.trim(),

      degree: formValue.degree.trim(),

      fieldOfStudy: formValue.fieldOfStudy.trim() || null,

      startYear,

      endYear,
    };

    // UPDATE

    if (this.editingEducationId) {
      this.resumeEducationService
        .update(this.resumeId, this.editingEducationId, request)
        .subscribe({
          next: () => {
            this.isEducationSaving = false;

            this.educationSuccessMessage = 'Education updated successfully.';

            this.editingEducationId = null;

            this.resetEducationForm();

            this.loadEducations();
          },

          error: (error) => {
            console.error('Failed to update education:', error);

            this.isEducationSaving = false;

            this.educationErrorMessage = 'Unable to update education.';
          },
        });

      return;
    }

    // CREATE

    this.resumeEducationService.create(this.resumeId, request).subscribe({
      next: () => {
        this.isEducationSaving = false;

        this.educationSuccessMessage = 'Education added successfully.';

        this.resetEducationForm();

        this.loadEducations();
      },

      error: (error) => {
        console.error('Failed to create education:', error);

        this.isEducationSaving = false;

        this.educationErrorMessage = 'Unable to add education.';
      },
    });
  }

  editEducation(education: ResumeEducation): void {
    this.editingEducationId = education.id;

    this.educationErrorMessage = '';
    this.educationSuccessMessage = '';

    this.educationForm.patchValue({
      institution: education.institution,

      degree: education.degree,

      fieldOfStudy: education.fieldOfStudy ?? '',

      startYear: education.startYear,

      endYear: education.endYear ?? null,
    });

    setTimeout(() => {
      document.getElementById('education-form')?.scrollIntoView({
        behavior: 'smooth',
      });
    }, 0);
  }

  cancelEducationEdit(): void {
    this.editingEducationId = null;

    this.resetEducationForm();

    this.educationErrorMessage = '';
    this.educationSuccessMessage = '';
  }

  deleteEducation(education: ResumeEducation): void {
    if (!this.resumeId) {
      return;
    }

    const confirmed = confirm(`Delete ${education.degree} from ${education.institution}?`);

    if (!confirmed) {
      return;
    }

    this.educationErrorMessage = '';
    this.educationSuccessMessage = '';

    this.resumeEducationService.delete(this.resumeId, education.id).subscribe({
      next: () => {
        this.educationSuccessMessage = 'Education deleted successfully.';

        if (this.editingEducationId === education.id) {
          this.editingEducationId = null;

          this.resetEducationForm();
        }

        this.loadEducations();
      },

      error: (error) => {
        console.error('Failed to delete education:', error);

        this.educationErrorMessage = 'Unable to delete education.';
      },
    });
  }

  resetEducationForm(): void {
    this.educationForm.reset({
      institution: '',
      degree: '',
      fieldOfStudy: '',
      startYear: new Date().getFullYear(),
      endYear: null,
    });
  }
}
