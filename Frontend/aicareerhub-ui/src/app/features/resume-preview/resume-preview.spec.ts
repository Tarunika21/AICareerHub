import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';

import { ResumePreview } from './resume-preview';

import { ResumeService } from '../../core/services/resume.service';
import { ResumeExperienceService } from '../../core/services/resume-experience.service';
import { ResumeEducationService } from '../../core/services/resume-education.service';
import { ResumeProjectService } from '../../core/services/resume-project.service';
import { AuthService } from '../../core/services/auth.service';

describe('ResumePreview', () => {
  let component: ResumePreview;
  let fixture: ComponentFixture<ResumePreview>;

  let resumeServiceSpy: jasmine.SpyObj<ResumeService>;
  let experienceServiceSpy: jasmine.SpyObj<ResumeExperienceService>;
  let educationServiceSpy: jasmine.SpyObj<ResumeEducationService>;
  let projectServiceSpy: jasmine.SpyObj<ResumeProjectService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  const resumeId = 'resume-123';

  beforeEach(async () => {
    resumeServiceSpy = jasmine.createSpyObj<ResumeService>(
      'ResumeService',
      ['getById']
    );

    experienceServiceSpy =
      jasmine.createSpyObj<ResumeExperienceService>(
        'ResumeExperienceService',
        ['getAll']
      );

    educationServiceSpy =
      jasmine.createSpyObj<ResumeEducationService>(
        'ResumeEducationService',
        ['getAll']
      );

    projectServiceSpy =
      jasmine.createSpyObj<ResumeProjectService>(
        'ResumeProjectService',
        ['getAll']
      );

    authServiceSpy = jasmine.createSpyObj<AuthService>(
      'AuthService',
      ['logout']
    );

    resumeServiceSpy.getById.and.returnValue(
      of({
        id: resumeId,
        title: 'Software Engineer',
        professionalSummary: 'Full-stack software engineer',
        skills: 'Angular, .NET, SQL',
      } as any)
    );

    experienceServiceSpy.getAll.and.returnValue(of([]));
    educationServiceSpy.getAll.and.returnValue(of([]));
    projectServiceSpy.getAll.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [ResumePreview],

      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) =>
                  key === 'id' ? resumeId : null,
              },
            },
          },
        },
        {
          provide: ResumeService,
          useValue: resumeServiceSpy,
        },
        {
          provide: ResumeExperienceService,
          useValue: experienceServiceSpy,
        },
        {
          provide: ResumeEducationService,
          useValue: educationServiceSpy,
        },
        {
          provide: ResumeProjectService,
          useValue: projectServiceSpy,
        },
        {
          provide: AuthService,
          useValue: authServiceSpy,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ResumePreview);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get resume id from route', () => {
    expect(component.resumeId).toBe(resumeId);
  });

  it('should load resume preview data on initialization', () => {
    expect(resumeServiceSpy.getById)
      .toHaveBeenCalledWith(resumeId);

    expect(experienceServiceSpy.getAll)
      .toHaveBeenCalledWith(resumeId);

    expect(educationServiceSpy.getAll)
      .toHaveBeenCalledWith(resumeId);

    expect(projectServiceSpy.getAll)
      .toHaveBeenCalledWith(resumeId);
  });

  it('should stop loading after all requests complete', () => {
    expect(component.isLoading).toBeFalse();
  });

  it('should set error message when resume loading fails', () => {
    resumeServiceSpy.getById.and.returnValue(
      throwError(() => new Error('Failed to load resume'))
    );

    component.loadResumePreview();

    expect(component.errorMessage)
      .toBe('Unable to load resume preview.');

    expect(component.isLoading).toBeFalse();
  });

  it('should call window.print when printResume is called', () => {
    const printSpy = spyOn(window, 'print');

    component.printResume();

    expect(printSpy).toHaveBeenCalled();
  });
});