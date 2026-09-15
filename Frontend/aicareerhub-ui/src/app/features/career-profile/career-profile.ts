import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Navbar } from '../../layout/navbar/navbar';

import { CareerProfileRequest } from '../../core/models/career-profile';

import { CareerProfileService } from '../../core/services/career-profile.service';

@Component({
  selector: 'app-career-profile',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './career-profile.html',
  styleUrl: './career-profile.css',
})
export class CareerProfile implements OnInit {
  isLoading = true;
  isSaving = false;

  profileExists = false;

  errorMessage = '';
  successMessage = '';

  profileForm;

  constructor(
    private fb: FormBuilder,
    private careerProfileService: CareerProfileService,
  ) {
    this.profileForm = this.fb.nonNullable.group({
      currentJobTitle: ['', [Validators.required, Validators.maxLength(100)]],

      yearsOfExperience: [0, [Validators.required, Validators.min(0), Validators.max(60)]],

      skills: ['', [Validators.required, Validators.maxLength(1000)]],

      currentLocation: ['', [Validators.required, Validators.maxLength(100)]],

      preferredLocations: ['', [Validators.required, Validators.maxLength(300)]],

      targetRole: ['', [Validators.required, Validators.maxLength(100)]],

      targetSalary: [0, [Validators.min(0)]],

      professionalSummary: ['', [Validators.maxLength(2000)]],
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.careerProfileService.get().subscribe({
      next: (profile) => {
        this.profileExists = true;

        this.profileForm.patchValue({
          currentJobTitle: profile.currentJobTitle,

          yearsOfExperience: profile.yearsOfExperience,

          skills: profile.skills,

          currentLocation: profile.currentLocation,

          preferredLocations: profile.preferredLocations,

          targetRole: profile.targetRole,

          targetSalary: profile.targetSalary ?? 0,

          professionalSummary: profile.professionalSummary ?? '',
        });

        this.isLoading = false;
      },

      error: (error) => {
        this.isLoading = false;

        /*
         * 404 is expected for a new user.
         * It simply means that the user has
         * not created a career profile yet.
         */
        if (error.status === 404) {
          this.profileExists = false;
          return;
        }

        console.error('Failed to load career profile:', error);

        this.errorMessage = 'Unable to load career profile.';
      },
    });
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();

      return;
    }

    this.isSaving = true;

    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.profileForm.getRawValue();

    const request: CareerProfileRequest = {
      currentJobTitle: formValue.currentJobTitle.trim(),

      yearsOfExperience: formValue.yearsOfExperience,

      skills: formValue.skills.trim(),

      currentLocation: formValue.currentLocation.trim(),

      preferredLocations: formValue.preferredLocations.trim(),

      targetRole: formValue.targetRole.trim(),

      targetSalary: formValue.targetSalary || null,

      professionalSummary: formValue.professionalSummary.trim() || null,
    };

    const request$ = this.profileExists
      ? this.careerProfileService.update(request)
      : this.careerProfileService.create(request);

    request$.subscribe({
      next: () => {
        this.isSaving = false;

        this.profileExists = true;

        this.successMessage = 'Career profile saved successfully.';
      },

      error: (error) => {
        console.error('Failed to save career profile:', error);

        this.isSaving = false;

        this.errorMessage = 'Unable to save career profile.';
      },
    });
  }
}
