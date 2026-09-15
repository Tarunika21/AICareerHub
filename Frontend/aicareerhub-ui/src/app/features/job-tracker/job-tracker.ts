import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Navbar } from '../../layout/navbar/navbar';
import { JobApplication } from '../../core/models/job-application';
import { JobApplicationService } from '../../core/services/job-application.service';

@Component({
  selector: 'app-job-tracker',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './job-tracker.html',
  styleUrl: './job-tracker.css',
})
export class JobTracker implements OnInit {
  applications: JobApplication[] = [];

  isLoading = true;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  editingApplicationId: string | null = null;

  jobForm;

  filterForm;

  constructor(
    private fb: FormBuilder,
    private jobApplicationService: JobApplicationService,
  ) {
    // Add / Edit Job Application Form
    this.jobForm = this.fb.nonNullable.group({
      companyName: ['', [Validators.required, Validators.maxLength(150)]],

      jobTitle: ['', [Validators.required, Validators.maxLength(150)]],

      jobUrl: [''],

      location: ['', [Validators.required, Validators.maxLength(150)]],

      status: ['Applied', Validators.required],

      appliedDate: ['', Validators.required],

      notes: ['', [Validators.maxLength(2000)]],
    });

    // Search / Filter Form
    this.filterForm = this.fb.nonNullable.group({
      search: [''],
      status: [''],
    });
  }

  ngOnInit(): void {
    this.loadApplications();
  }

  // ------------------------------------------------
  // LOAD / SEARCH / FILTER
  // ------------------------------------------------

  loadApplications(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const search = this.filterForm.controls.search.value.trim();

    const status = this.filterForm.controls.status.value;

    this.jobApplicationService.getAll(status || undefined, search || undefined).subscribe({
      next: (applications) => {
        this.applications = applications;
        this.isLoading = false;
      },

      error: (error) => {
        console.error('Failed to load job applications:', error);

        this.errorMessage = 'Unable to load job applications.';

        this.isLoading = false;
      },
    });
  }

  applyFilters(): void {
    this.loadApplications();
  }

  clearFilters(): void {
    this.filterForm.reset({
      search: '',
      status: '',
    });

    this.loadApplications();
  }

  // ------------------------------------------------
  // CREATE / UPDATE
  // ------------------------------------------------

  saveApplication(): void {
    if (this.jobForm.invalid) {
      this.jobForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;

    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.jobForm.getRawValue();

    /*
     * Optional text fields are converted to null.
     *
     * This is especially important for jobUrl because
     * the backend uses [Url].
     *
     * Sending "" would fail URL validation.
     */
    const request = {
      ...formValue,

      jobUrl: formValue.jobUrl.trim() || null,

      notes: formValue.notes.trim() || null,
    };

    // UPDATE EXISTING APPLICATION
    if (this.editingApplicationId) {
      this.jobApplicationService.update(this.editingApplicationId, request).subscribe({
        next: () => {
          this.isSaving = false;

          this.successMessage = 'Job application updated successfully.';

          this.editingApplicationId = null;

          this.resetForm();

          this.loadApplications();
        },

        error: (error) => {
          console.error('Failed to update job application:', error);

          this.isSaving = false;

          this.errorMessage = 'Unable to update job application.';
        },
      });

      return;
    }

    // CREATE NEW APPLICATION
    this.jobApplicationService.create(request).subscribe({
      next: () => {
        this.isSaving = false;

        this.successMessage = 'Job application added successfully.';

        this.resetForm();

        this.loadApplications();
      },

      error: (error) => {
        console.error('Failed to add job application:', error);

        this.isSaving = false;

        this.errorMessage = 'Unable to add job application.';
      },
    });
  }

  // ------------------------------------------------
  // EDIT
  // ------------------------------------------------

  editApplication(application: JobApplication): void {
    this.editingApplicationId = application.id;

    this.errorMessage = '';
    this.successMessage = '';

    this.jobForm.setValue({
      companyName: application.companyName,

      jobTitle: application.jobTitle,

      jobUrl: application.jobUrl ?? '',

      location: application.location,

      status: application.status,

      appliedDate: application.appliedDate,

      notes: application.notes ?? '',
    });

    // Move user back to the form
    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  }

  cancelEdit(): void {
    this.editingApplicationId = null;

    this.resetForm();

    this.errorMessage = '';
    this.successMessage = '';
  }

  // ------------------------------------------------
  // DELETE
  // ------------------------------------------------

  deleteApplication(application: JobApplication): void {
    const confirmed = window.confirm(
      `Are you sure you want to delete the application for ${application.jobTitle} at ${application.companyName}?`,
    );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.jobApplicationService.delete(application.id).subscribe({
      next: () => {
        this.successMessage = 'Job application deleted successfully.';

        // If the application currently being edited
        // was deleted, reset the form.
        if (this.editingApplicationId === application.id) {
          this.editingApplicationId = null;

          this.resetForm();
        }

        this.loadApplications();
      },

      error: (error) => {
        console.error('Failed to delete job application:', error);

        this.errorMessage = 'Unable to delete job application.';
      },
    });
  }

  // ------------------------------------------------
  // FORM RESET
  // ------------------------------------------------

  resetForm(): void {
    this.jobForm.reset({
      companyName: '',

      jobTitle: '',

      jobUrl: '',

      location: '',

      status: 'Applied',

      appliedDate: '',

      notes: '',
    });
  }
}
