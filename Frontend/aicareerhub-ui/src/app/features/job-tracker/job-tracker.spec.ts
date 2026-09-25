import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { JobTracker } from './job-tracker';
import { JobApplicationService } from '../../core/services/job-application.service';

describe('JobTracker', () => {
  let component: JobTracker;
  let fixture: ComponentFixture<JobTracker>;

  let jobApplicationServiceSpy: jasmine.SpyObj<JobApplicationService>;

  beforeEach(async () => {
    jobApplicationServiceSpy =
      jasmine.createSpyObj<JobApplicationService>(
        'JobApplicationService',
        [
          'getAll',
          'create',
          'update',
          'delete',
        ],
      );

    jobApplicationServiceSpy.getAll.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [JobTracker],

      providers: [
        // Navbar -> AuthService -> HttpClient
        provideHttpClient(),

        // Navbar -> RouterLink -> ActivatedRoute / Router
        provideRouter([]),

        // Mock JobApplicationService so no real API calls are made
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
    expect(jobApplicationServiceSpy.getAll)
      .toHaveBeenCalledWith(undefined, undefined);

    expect(component.applications).toEqual([]);
    expect(component.isLoading).toBeFalse();
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

    expect(jobApplicationServiceSpy.create)
      .not.toHaveBeenCalled();

    expect(jobApplicationServiceSpy.update)
      .not.toHaveBeenCalled();
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

    expect(jobApplicationServiceSpy.getAll)
      .toHaveBeenCalledWith(undefined, undefined);
  });
});