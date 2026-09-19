import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../layout/navbar/navbar';
import { Resume, ResumeRequest } from '../../core/models/resume';
import { ResumeExperience, ResumeExperienceRequest } from '../../core/models/resume-experience';
import { ResumeService } from '../../core/services/resume.service';
import { ResumeExperienceService } from '../../core/services/resume-experience.service';
import { ResumeEducation, ResumeEducationRequest } from '../../core/models/resume-education';
import { ResumeEducationService } from '../../core/services/resume-education.service';
import { ResumeProject, ResumeProjectRequest } from '../../core/models/resume-project';
import { AiService } from '../../core/services/ai.service';
import { CareerProfileService } from '../../core/services/career-profile.service';
import { ResumeProjectService } from '../../core/services/resume-project.service';

@Component({
  selector: 'app-resume-builder',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink, Navbar],
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
  // PROJECTS
  // ==============================

  projects: ResumeProject[] = [];

  isProjectLoading = false;
  isProjectSaving = false;

  projectErrorMessage = '';
  projectSuccessMessage = '';

  editingProjectId: string | null = null;

  // ==============================
  // AI - RESUME SUMMARY
  // ==============================

  targetRole = '';

  isSummaryAiLoading = false;

  summaryAiErrorMessage = '';

  suggestedSummary: string | null = null;

  // ==============================
  // AI - EXPERIENCE
  // ==============================

  isExperienceAiLoading = false;

  experienceAiErrorMessage = '';

  suggestedExperienceDescription: string | null = null;

  // ==============================
  // AI - RESUME SUGGESTIONS
  // ==============================

  jobDescription = '';

  isResumeSuggestionsLoading = false;

  resumeSuggestionsErrorMessage = '';

  resumeSuggestions: string[] = [];

  // ==============================
  // RESUME FORM
  // ==============================

  resumeForm;

  // ==============================
  // EXPERIENCE FORM
  // ==============================

  experienceForm;
  educationForm;
  projectForm;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private resumeService: ResumeService,
    private resumeExperienceService: ResumeExperienceService,
    private resumeEducationService: ResumeEducationService,
    private resumeProjectService: ResumeProjectService,
    private aiService: AiService,
    private careerProfileService: CareerProfileService,
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

    this.projectForm = this.fb.nonNullable.group({
      name: ['', [Validators.required, Validators.maxLength(150)]],

      description: ['', [Validators.required, Validators.maxLength(2000)]],

      technologies: ['', [Validators.required, Validators.maxLength(1000)]],

      projectUrl: ['', [Validators.maxLength(500), Validators.pattern(/^https?:\/\/.+/i)]],
    });
  }

  // ==============================
  // INITIALIZATION
  // ==============================

  ngOnInit(): void {
    this.loadCareerProfile();
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.resumeId = id;
      this.loadResume();
      this.loadExperiences();
      this.loadEducations();
      this.loadProjects();
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
    this.suggestedExperienceDescription = null;
    this.experienceAiErrorMessage = '';
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
    this.suggestedExperienceDescription = null;
    this.experienceAiErrorMessage = '';
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

  loadProjects(): void {
    if (!this.resumeId) {
      return;
    }

    this.isProjectLoading = true;
    this.projectErrorMessage = '';

    this.resumeProjectService.getAll(this.resumeId).subscribe({
      next: (projects) => {
        this.projects = projects;
        this.isProjectLoading = false;
      },

      error: (error) => {
        console.error('Failed to load projects:', error);
        this.projectErrorMessage = 'Unable to load projects.';
        this.isProjectLoading = false;
      },
    });
  }

  saveProject(): void {
    if (!this.resumeId) {
      this.projectErrorMessage = 'Save Basic Details first.';
      return;
    }

    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    this.isProjectSaving = true;
    this.projectErrorMessage = '';
    this.projectSuccessMessage = '';
    const formValue = this.projectForm.getRawValue();

    const request: ResumeProjectRequest = {
      name: formValue.name.trim(),
      description: formValue.description.trim(),
      technologies: formValue.technologies.trim(),
      projectUrl: formValue.projectUrl.trim() || null,
    };

    // ============================
    // UPDATE PROJECT
    // ============================

    if (this.editingProjectId) {
      this.resumeProjectService.update(this.resumeId, this.editingProjectId, request).subscribe({
        next: () => {
          this.isProjectSaving = false;
          this.projectSuccessMessage = 'Project updated successfully.';
          this.editingProjectId = null;
          this.resetProjectForm();
          this.loadProjects();
        },

        error: (error) => {
          console.error('Failed to update project:', error);
          this.isProjectSaving = false;
          this.projectErrorMessage = 'Unable to update project.';
        },
      });

      return;
    }

    // ============================
    // CREATE PROJECT
    // ============================

    this.resumeProjectService.create(this.resumeId, request).subscribe({
      next: () => {
        this.isProjectSaving = false;
        this.projectSuccessMessage = 'Project added successfully.';
        this.resetProjectForm();
        this.loadProjects();
      },

      error: (error) => {
        console.error('Failed to create project:', error);
        this.isProjectSaving = false;
        this.projectErrorMessage = 'Unable to add project.';
      },
    });
  }

  editProject(project: ResumeProject): void {
    this.editingProjectId = project.id;
    this.projectErrorMessage = '';
    this.projectSuccessMessage = '';
    this.projectForm.patchValue({
      name: project.name,
      description: project.description,
      technologies: project.technologies,
      projectUrl: project.projectUrl ?? '',
    });

    setTimeout(() => {
      document.getElementById('project-form')?.scrollIntoView({
        behavior: 'smooth',
      });
    }, 0);
  }

  cancelProjectEdit(): void {
    this.editingProjectId = null;
    this.resetProjectForm();
    this.projectErrorMessage = '';
    this.projectSuccessMessage = '';
  }

  deleteProject(project: ResumeProject): void {
    if (!this.resumeId) {
      return;
    }

    const confirmed = confirm(`Delete project "${project.name}"?`);

    if (!confirmed) {
      return;
    }

    this.projectErrorMessage = '';
    this.projectSuccessMessage = '';

    this.resumeProjectService.delete(this.resumeId, project.id).subscribe({
      next: () => {
        this.projectSuccessMessage = 'Project deleted successfully.';

        if (this.editingProjectId === project.id) {
          this.editingProjectId = null;
          this.resetProjectForm();
        }
        this.loadProjects();
      },

      error: (error) => {
        console.error('Failed to delete project:', error);

        this.projectErrorMessage = 'Unable to delete project.';
      },
    });
  }

  resetProjectForm(): void {
    this.projectForm.reset({
      name: '',
      description: '',
      technologies: '',
      projectUrl: '',
    });
  }

  loadCareerProfile(): void {
    this.careerProfileService.get().subscribe({
      next: (profile) => {
        this.targetRole = profile.targetRole;
      },

      error: (error) => {
        console.error('Failed to load career profile:', error);

        /*
         * We don't show a general page error here.
         * The Resume Builder should still work even
         * if the user has no Career Profile.
         */
        this.targetRole = '';
      },
    });
  }

  improveSummary(): void {
    this.summaryAiErrorMessage = '';
    this.suggestedSummary = null;
    const formValue = this.resumeForm.getRawValue();
    const currentSummary = formValue.professionalSummary.trim();
    const skills = formValue.skills.trim();

    if (!currentSummary) {
      this.summaryAiErrorMessage = 'Enter a professional summary before using AI improvement.';
      return;
    }

    if (!skills) {
      this.summaryAiErrorMessage = 'Add your skills before using AI improvement.';
      return;
    }

    if (!this.targetRole) {
      this.summaryAiErrorMessage =
        'Add a target role in your Career Profile before using AI improvement.';

      return;
    }
    this.isSummaryAiLoading = true;
    this.aiService
      .improveResumeSummary({
        currentSummary,
        targetRole: this.targetRole,
        skills,
      })
      .subscribe({
        next: (response) => {
          this.suggestedSummary = response.improvedSummary;
          this.isSummaryAiLoading = false;
        },

        error: (error) => {
          console.error('Failed to improve summary:', error);
          this.summaryAiErrorMessage = 'Unable to generate a summary suggestion.';
          this.isSummaryAiLoading = false;
        },
      });
  }

  useSuggestedSummary(): void {
    if (!this.suggestedSummary) {
      return;
    }

    this.resumeForm.patchValue({
      professionalSummary: this.suggestedSummary,
    });

    this.suggestedSummary = null;
    this.summaryAiErrorMessage = '';
  }

  discardSuggestedSummary(): void {
    this.suggestedSummary = null;
    this.summaryAiErrorMessage = '';
  }

  improveExperienceDescription(): void {
    this.experienceAiErrorMessage = '';
    this.suggestedExperienceDescription = null;

    const formValue = this.experienceForm.getRawValue();

    const jobTitle = formValue.jobTitle.trim();

    const company = formValue.company.trim();

    const currentDescription = formValue.description.trim();

    if (!jobTitle) {
      this.experienceAiErrorMessage = 'Enter a job title before using AI improvement.';

      return;
    }

    if (!company) {
      this.experienceAiErrorMessage = 'Enter a company before using AI improvement.';

      return;
    }

    if (!currentDescription) {
      this.experienceAiErrorMessage =
        'Enter an experience description before using AI improvement.';

      return;
    }

    this.isExperienceAiLoading = true;

    this.aiService
      .improveExperience({
        jobTitle,

        company,

        currentDescription,

        /*
         * Our Experience model does not currently
         * have its own Technologies field.
         *
         * Sending null is valid because the backend
         * DTO defines Technologies as optional.
         */
        technologies: null,
      })
      .subscribe({
        next: (response) => {
          this.suggestedExperienceDescription = response.improvedDescription;

          this.isExperienceAiLoading = false;
        },

        error: (error) => {
          console.error('Failed to improve experience:', error);

          this.experienceAiErrorMessage = 'Unable to generate an experience suggestion.';

          this.isExperienceAiLoading = false;
        },
      });
  }

  useSuggestedExperienceDescription(): void {
    if (!this.suggestedExperienceDescription) {
      return;
    }

    this.experienceForm.patchValue({
      description: this.suggestedExperienceDescription,
    });

    this.suggestedExperienceDescription = null;
    this.experienceAiErrorMessage = '';
  }

  discardSuggestedExperienceDescription(): void {
    this.suggestedExperienceDescription = null;
    this.experienceAiErrorMessage = '';
  }

  getResumeSuggestions(): void {
    this.resumeSuggestionsErrorMessage = '';
    this.resumeSuggestions = [];

    if (!this.resumeId || !this.resume) {
      this.resumeSuggestionsErrorMessage = 'Save the resume before requesting suggestions.';

      return;
    }

    const jobDescription = this.jobDescription.trim();

    if (!jobDescription) {
      this.resumeSuggestionsErrorMessage = 'Paste a job description before requesting suggestions.';

      return;
    }

    const resumeContent = this.buildResumeContent();

    if (!resumeContent.trim()) {
      this.resumeSuggestionsErrorMessage = 'Add some resume content before requesting suggestions.';

      return;
    }

    this.isResumeSuggestionsLoading = true;

    this.aiService
      .getResumeSuggestions({
        resumeContent,
        jobDescription,
      })
      .subscribe({
        next: (response) => {
          this.resumeSuggestions = response.suggestions;

          this.isResumeSuggestionsLoading = false;
        },

        error: (error) => {
          console.error('Failed to get resume suggestions:', error);

          this.resumeSuggestionsErrorMessage = 'Unable to generate resume suggestions.';

          this.isResumeSuggestionsLoading = false;
        },
      });
  }

  private buildResumeContent(): string {
    if (!this.resume) {
      return '';
    }

    const sections: string[] = [];

    sections.push(`Resume Title: ${this.resume.title}`);

    if (this.resume.professionalSummary) {
      sections.push(`Professional Summary: ${this.resume.professionalSummary}`);
    }

    if (this.resume.skills) {
      sections.push(`Skills: ${this.resume.skills}`);
    }

    if (this.experiences.length > 0) {
      const experienceContent = this.experiences
        .map((experience) => {
          return [
            `Job Title: ${experience.jobTitle}`,
            `Company: ${experience.company}`,
            `Description: ${experience.description ?? ''}`,
          ].join('\n');
        })
        .join('\n\n');

      sections.push(`Work Experience:\n${experienceContent}`);
    }

    if (this.educations.length > 0) {
      const educationContent = this.educations
        .map((education) => {
          return [
            `Institution: ${education.institution}`,
            `Degree: ${education.degree}`,
            `Field of Study: ${education.fieldOfStudy ?? ''}`,
            `Start Year: ${education.startYear}`,
            `End Year: ${education.endYear ?? 'Present'}`,
          ].join('\n');
        })
        .join('\n\n');

      sections.push(`Education:\n${educationContent}`);
    }

    if (this.projects.length > 0) {
      const projectContent = this.projects
        .map((project) => {
          return [
            `Project: ${project.name}`,
            `Description: ${project.description}`,
            `Technologies: ${project.technologies}`,
          ].join('\n');
        })
        .join('\n\n');

      sections.push(`Projects:\n${projectContent}`);
    }

    return sections.join('\n\n');
  }
}
