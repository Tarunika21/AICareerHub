import { CommonModule } from '@angular/common';

import { Component, OnInit } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Navbar } from '../../layout/navbar/navbar';

import { Resume, ResumeRequest } from '../../core/models/resume';

import { ResumeService } from '../../core/services/resume.service';

@Component({
  selector: 'app-resumes',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './resumes.html',
  styleUrl: './resumes.css',
})
export class Resumes implements OnInit {
  resumes: Resume[] = [];

  isLoading = true;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  editingResumeId: string | null = null;

  resumeForm;

  constructor(
    private fb: FormBuilder,
    private resumeService: ResumeService,
  ) {
    this.resumeForm = this.fb.nonNullable.group({
      title: ['', [Validators.required, Validators.maxLength(150)]],

      professionalSummary: ['', [Validators.maxLength(2000)]],

      skills: ['', [Validators.maxLength(1000)]],
    });
  }

  ngOnInit(): void {
    this.loadResumes();
  }

  // ---------------------------------------------
  // LOAD
  // ---------------------------------------------

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

  // ---------------------------------------------
  // CREATE / UPDATE
  // ---------------------------------------------

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

    // UPDATE
    if (this.editingResumeId) {
      this.resumeService.update(this.editingResumeId, request).subscribe({
        next: () => {
          this.isSaving = false;

          this.successMessage = 'Resume updated successfully.';

          this.editingResumeId = null;

          this.resetForm();

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

    // CREATE
    this.resumeService.create(request).subscribe({
      next: () => {
        this.isSaving = false;

        this.successMessage = 'Resume created successfully.';

        this.resetForm();

        this.loadResumes();
      },

      error: (error) => {
        console.error('Failed to create resume:', error);

        this.isSaving = false;

        this.errorMessage = 'Unable to create resume.';
      },
    });
  }

  // ---------------------------------------------
  // EDIT
  // ---------------------------------------------

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

    this.resetForm();

    this.errorMessage = '';
    this.successMessage = '';
  }

  // ---------------------------------------------
  // DELETE
  // ---------------------------------------------

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

          this.resetForm();
        }

        this.loadResumes();
      },

      error: (error) => {
        console.error('Failed to delete resume:', error);

        this.errorMessage = 'Unable to delete resume.';
      },
    });
  }

  // ---------------------------------------------
  // RESET
  // ---------------------------------------------

  resetForm(): void {
    this.resumeForm.reset({
      title: '',

      professionalSummary: '',

      skills: '',
    });
  }
}
