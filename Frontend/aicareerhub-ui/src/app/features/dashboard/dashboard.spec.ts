import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import {
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { of } from 'rxjs';

import { Dashboard } from './dashboard';

import { JobApplicationService } from '../../core/services/job-application.service';
import { CareerProfileService } from '../../core/services/career-profile.service';
import { ResumeService } from '../../core/services/resume.service';

describe('Dashboard', () => {
  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;

  const jobApplicationServiceMock = {
    getStats: jasmine.createSpy('getStats').and.returnValue(
      of({
        total: 0,
        applied: 0,
        interview: 0,
        offer: 0,
        rejected: 0,
        withdrawn: 0,
      }),
    ),
  };

  const careerProfileServiceMock = {
    get: jasmine.createSpy('get').and.returnValue(of(null)),
  };

  const resumeServiceMock = {
    getAll: jasmine.createSpy('getAll').and.returnValue(of([])),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Dashboard],

      providers: [
        provideRouter([]),

        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),

        {
          provide: JobApplicationService,
          useValue: jobApplicationServiceMock,
        },

        {
          provide: CareerProfileService,
          useValue: careerProfileServiceMock,
        },

        {
          provide: ResumeService,
          useValue: resumeServiceMock,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});