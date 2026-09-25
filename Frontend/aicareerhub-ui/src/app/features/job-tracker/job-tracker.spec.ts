import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { JobTracker } from './job-tracker';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobApplication } from '../../core/models/job-application';

describe('JobTracker', () => {
  let component: JobTracker;
  let fixture: ComponentFixture<JobTracker>;
  let jobApplicationServiceSpy: jasmine.SpyObj<JobApplicationService>;

  const mockApplication: JobApplication = {
    id: 'job-1',
    companyName: 'Microsoft',
    jobTitle: 'Software Engineer',
    jobUrl: 'https://example.com/job',
    location: 'Bangalore',
    status: 'Applied',
    appliedDate: '2026-09-20',
    notes: 'Applied through careers page',
    createdAt: '2026-09-20T00:00:00.000Z',
    updatedAt: '2026-09-20T00:00:00.000Z',
  };

  beforeEach(async () => {
    jobApplicationServiceSpy = jasmine.createSpyObj<JobApplicationService>(
      'JobApplicationService',
      ['getAll', 'create', 'update', 'delete'],
    );
    jobApplicationServiceSpy.getAll.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [JobTracker],
      providers: [
        provideHttpClient(),
        provideRouter([]),
        {
          provide: JobApplicationService,
          useValue: jobApplicationServiceSpy,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(JobTracker);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load applications on initialization', () => {
    expect(jobApplicationServiceSpy.getAll).toHaveBeenCalledWith(
      undefined,
      undefined,
    );
    expect(component.applications).toEqual([]);
    expect(component.isLoading).toBeFalse();
  });

  it('should load applications returned by service', () => {
    jobApplicationServiceSpy.getAll.and.returnValue(of([mockApplication]));
    component.loadApplications();
    expect(component.applications).toEqual([mockApplication]);
    expect(component.isLoading).toBeFalse();
    expect(component.errorMessage).toBe('');
  });

  it('should set error message when loading applications fails', () => {
    jobApplicationServiceSpy.getAll.and.returnValue(
      throwError(() => new Error('Load failed')),
    );
    component.loadApplications();
    expect(component.errorMessage).toBe(
      'Unable to load job applications.',
    );
    expect(component.isLoading).toBeFalse();
  });

  it('should apply search and status filters', () => {
    component.filterForm.setValue({
      search: ' Microsoft ',
      status: 'Applied',
    });
    jobApplicationServiceSpy.getAll.calls.reset();
    component.applyFilters();
    expect(jobApplicationServiceSpy.getAll).toHaveBeenCalledWith(
      'Applied',
      'Microsoft',
    );
  });

  it('should clear filters and reload applications', () => {
    component.filterForm.setValue({
      search: 'Microsoft',
      status: 'Applied',
    });
    jobApplicationServiceSpy.getAll.calls.reset();
    component.clearFilters();
    expect(component.filterForm.getRawValue()).toEqual({
      search: '',
      status: '',
    });
    expect(jobApplicationServiceSpy.getAll).toHaveBeenCalledWith(
      undefined,
      undefined,
    );
  });

  it('should not save when form is invalid', () => {
    component.jobForm.reset({
      companyName: '',
      jobTitle: '',
      jobUrl: '',
      location: '',
      status: 'Applied',
      appliedDate: '',
      notes: '',
    });
    component.saveApplication();
    expect(jobApplicationServiceSpy.create).not.toHaveBeenCalled();
    expect(jobApplicationServiceSpy.update).not.toHaveBeenCalled();
  });

  it('should reject invalid job URL', () => {
    component.jobForm.controls.jobUrl.setValue('invalid-url');
    component.jobForm.controls.jobUrl.markAsTouched();
    expect(component.jobForm.controls.jobUrl.invalid).toBeTrue();
    expect(
      component.jobForm.controls.jobUrl.hasError('pattern'),
    ).toBeTrue();
  });

  it('should accept valid http job URL', () => {
    component.jobForm.controls.jobUrl.setValue(
      'http://example.com/job',
    );
    expect(component.jobForm.controls.jobUrl.valid).toBeTrue();
  });

  it('should accept valid https job URL', () => {
    component.jobForm.controls.jobUrl.setValue(
      'https://example.com/job',
    );
    expect(component.jobForm.controls.jobUrl.valid).toBeTrue();
  });

  it('should create a job application', () => {
    jobApplicationServiceSpy.create.and.returnValue(of(mockApplication));
    component.jobForm.setValue({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: 'https://example.com/job',
      location: 'Bangalore',
      status: 'Applied',
      appliedDate: '2026-09-20',
      notes: 'Applied through careers page',
    });
    component.saveApplication();
    expect(jobApplicationServiceSpy.create).toHaveBeenCalledWith({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: 'https://example.com/job',
      location: 'Bangalore',
      status: 'Applied',
      appliedDate: '2026-09-20',
      notes: 'Applied through careers page',
    });
    expect(component.successMessage).toBe(
      'Job application added successfully.',
    );
    expect(component.isSaving).toBeFalse();
  });

  it('should convert empty optional fields to null when creating', () => {
    jobApplicationServiceSpy.create.and.returnValue(of(mockApplication));
    component.jobForm.setValue({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: '',
      location: 'Bangalore',
      status: 'Applied',
      appliedDate: '2026-09-20',
      notes: '',
    });
    component.saveApplication();
    expect(jobApplicationServiceSpy.create).toHaveBeenCalledWith({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: null,
      location: 'Bangalore',
      status: 'Applied',
      appliedDate: '2026-09-20',
      notes: null,
    });
  });

  it('should set error message when create fails', () => {
    jobApplicationServiceSpy.create.and.returnValue(
      throwError(() => new Error('Create failed')),
    );
    component.jobForm.setValue({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: '',
      location: 'Bangalore',
      status: 'Applied',
      appliedDate: '2026-09-20',
      notes: '',
    });
    component.saveApplication();
    expect(component.errorMessage).toBe(
      'Unable to add job application.',
    );
    expect(component.isSaving).toBeFalse();
  });

  it('should populate form when editing an application', () => {
    spyOn(window, 'scrollTo');
    component.editApplication(mockApplication);
    expect(component.editingApplicationId).toBe('job-1');
    expect(component.jobForm.getRawValue()).toEqual({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: 'https://example.com/job',
      location: 'Bangalore',
      status: 'Applied',
      appliedDate: '2026-09-20',
      notes: 'Applied through careers page',
    });
    expect(window.scrollTo).toHaveBeenCalled();
  });

  it('should update an existing application', () => {
    jobApplicationServiceSpy.update.and.returnValue(of(mockApplication));
    component.editingApplicationId = 'job-1';
    component.jobForm.setValue({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer II',
      jobUrl: 'https://example.com/job',
      location: 'Bangalore',
      status: 'Interview',
      appliedDate: '2026-09-20',
      notes: 'Interview scheduled',
    });
    component.saveApplication();
    expect(jobApplicationServiceSpy.update).toHaveBeenCalledWith(
      'job-1',
      {
        companyName: 'Microsoft',
        jobTitle: 'Software Engineer II',
        jobUrl: 'https://example.com/job',
        location: 'Bangalore',
        status: 'Interview',
        appliedDate: '2026-09-20',
        notes: 'Interview scheduled',
      },
    );
    expect(component.successMessage).toBe(
      'Job application updated successfully.',
    );
    expect(component.editingApplicationId).toBeNull();
    expect(component.isSaving).toBeFalse();
  });

  it('should set error message when update fails', () => {
    jobApplicationServiceSpy.update.and.returnValue(
      throwError(() => new Error('Update failed')),
    );
    component.editingApplicationId = 'job-1';
    component.jobForm.setValue({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: '',
      location: 'Bangalore',
      status: 'Interview',
      appliedDate: '2026-09-20',
      notes: '',
    });
    component.saveApplication();
    expect(component.errorMessage).toBe(
      'Unable to update job application.',
    );
    expect(component.isSaving).toBeFalse();
  });

  it('should cancel edit and reset form', () => {
    component.editingApplicationId = 'job-1';
    component.jobForm.patchValue({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
    });
    component.errorMessage = 'Error';
    component.successMessage = 'Success';
    component.cancelEdit();
    expect(component.editingApplicationId).toBeNull();
    expect(component.errorMessage).toBe('');
    expect(component.successMessage).toBe('');
    expect(component.jobForm.getRawValue()).toEqual({
      companyName: '',
      jobTitle: '',
      jobUrl: '',
      location: '',
      status: 'Applied',
      appliedDate: '',
      notes: '',
    });
  });

  it('should delete application when user confirms', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    jobApplicationServiceSpy.delete.and.returnValue(of(void 0));
    component.deleteApplication(mockApplication);
    expect(window.confirm).toHaveBeenCalled();
    expect(jobApplicationServiceSpy.delete).toHaveBeenCalledWith(
      'job-1',
    );
    expect(component.successMessage).toBe(
      'Job application deleted successfully.',
    );
  });

  it('should not delete application when user cancels confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(false);
    component.deleteApplication(mockApplication);
    expect(jobApplicationServiceSpy.delete).not.toHaveBeenCalled();
  });

  it('should reset edit state when currently edited application is deleted', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    jobApplicationServiceSpy.delete.and.returnValue(of(void 0));
    component.editingApplicationId = 'job-1';
    component.deleteApplication(mockApplication);
    expect(component.editingApplicationId).toBeNull();
    expect(component.jobForm.controls.status.value).toBe('Applied');
    expect(component.jobForm.controls.companyName.value).toBe('');
  });

  it('should set error message when delete fails', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    jobApplicationServiceSpy.delete.and.returnValue(
      throwError(() => new Error('Delete failed')),
    );
    component.deleteApplication(mockApplication);
    expect(component.errorMessage).toBe(
      'Unable to delete job application.',
    );
  });

  it('should reset job form to default values', () => {
    component.jobForm.setValue({
      companyName: 'Microsoft',
      jobTitle: 'Software Engineer',
      jobUrl: 'https://example.com/job',
      location: 'Bangalore',
      status: 'Interview',
      appliedDate: '2026-09-20',
      notes: 'Test notes',
    });
    component.resetForm();
    expect(component.jobForm.getRawValue()).toEqual({
      companyName: '',
      jobTitle: '',
      jobUrl: '',
      location: '',
      status: 'Applied',
      appliedDate: '',
      notes: '',
    });
  });
});