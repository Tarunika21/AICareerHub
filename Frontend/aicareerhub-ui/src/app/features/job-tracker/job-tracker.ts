import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Navbar } from '../../layout/navbar/navbar';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobApplication } from '../../core/models/job-application';

@Component({
  selector: 'app-job-tracker',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './job-tracker.html',
  styleUrl: './job-tracker.css'
})
export class JobTracker implements OnInit {

  applications: JobApplication[] = [];

  isLoading = true;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  jobForm;

  constructor(
    private fb: FormBuilder,
    private jobApplicationService: JobApplicationService
  ) {
    this.jobForm = this.fb.nonNullable.group({
      companyName: ['', [
        Validators.required,
        Validators.maxLength(150)
      ]],
      jobTitle: ['', [
        Validators.required,
        Validators.maxLength(150)
      ]],
      jobUrl: [''],
      location: ['', [
        Validators.required,
        Validators.maxLength(150)
      ]],
      status: ['Applied', Validators.required],
      appliedDate: ['', Validators.required],
      notes: ['', Validators.maxLength(2000)]
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
        this.errorMessage =
          'Unable to load job applications.';
        this.isLoading = false;
      }
    });
  }

  addApplication(): void {
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
    notes: formValue.notes.trim() || null
  };

  this.jobApplicationService
    .create(request)
    .subscribe({
      next: () => {
        this.isSaving = false;
        this.successMessage =
          'Job application added successfully.';

        this.jobForm.reset({
          companyName: '',
          jobTitle: '',
          jobUrl: '',
          location: '',
          status: 'Applied',
          appliedDate: '',
          notes: ''
        });

        this.loadApplications();
      },
      error: (error) => {
        console.error(
          'Failed to add job application:',
          error
        );

        this.isSaving = false;
        this.errorMessage =
          'Unable to add job application.';
      }
    });
}
}