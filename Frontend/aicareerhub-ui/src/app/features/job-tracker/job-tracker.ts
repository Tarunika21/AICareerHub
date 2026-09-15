import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Navbar } from '../../layout/navbar/navbar';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobApplication } from '../../core/models/job-application';

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

  constructor(
    private fb: FormBuilder,
    private jobApplicationService: JobApplicationService,
  ) {
    this.jobForm = this.fb.nonNullable.group({
      companyName: ['', [Validators.required, Validators.maxLength(150)]],
      jobTitle: ['', [Validators.required, Validators.maxLength(150)]],
      jobUrl: [''],
      location: ['', [Validators.required, Validators.maxLength(150)]],
      status: ['Applied', Validators.required],
      appliedDate: ['', Validators.required],
      notes: ['', Validators.maxLength(2000)],
    });
  }

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
        this.errorMessage = 'Unable to load job applications.';
        this.isLoading = false;
      },
    });
  }

  saveApplication(): void {
    if (this.jobForm.invalid) {
      this.jobForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.jobForm.getRawValue();

    const request = {
      ...formValue,
      jobUrl: formValue.jobUrl.trim() || null,
      notes: formValue.notes.trim() || null,
    };

    if (this.editingApplicationId) {
      this.jobApplicationService.update(this.editingApplicationId, request).subscribe({
        next: () => {
          this.isSaving = false;

          this.successMessage = 'Job application updated successfully.';

          this.editingApplicationId = null;

          this.resetForm();

          this.loadApplications();
        },
        error: () => {
          this.isSaving = false;

          this.errorMessage = 'Unable to update job application.';
        },
      });
    } else {
      this.jobApplicationService.create(request).subscribe({
        next: () => {
          this.isSaving = false;

          this.successMessage = 'Job application added successfully.';

          this.resetForm();

          this.loadApplications();
        },
        error: () => {
          this.isSaving = false;

          this.errorMessage = 'Unable to add job application.';
        },
      });
    }
  }

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
  editApplication(application: JobApplication): void {
    this.editingApplicationId = application.id;

    this.successMessage = '';
    this.errorMessage = '';

    this.jobForm.setValue({
      companyName: application.companyName,
      jobTitle: application.jobTitle,
      jobUrl: application.jobUrl ?? '',
      location: application.location,
      status: application.status,
      appliedDate: application.appliedDate,
      notes: application.notes ?? '',
    });

    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  }

  cancelEdit(): void {
    this.editingApplicationId = null;

    this.jobForm.reset({
      companyName: '',
      jobTitle: '',
      jobUrl: '',
      location: '',
      status: 'Applied',
      appliedDate: '',
      notes: '',
    });

    this.errorMessage = '';
    this.successMessage = '';
  }

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

        // If the deleted application was being edited,
        // reset the form as well.
        if (this.editingApplicationId === application.id) {
          this.editingApplicationId = null;
          this.resetForm();
        }

        this.loadApplications();
      },
      error: () => {
        this.errorMessage = 'Unable to delete job application.';
      },
    });
  }
}
