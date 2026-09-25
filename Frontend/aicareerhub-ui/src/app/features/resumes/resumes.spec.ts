import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { Resumes } from './resumes';
import { ResumeService } from '../../core/services/resume.service';
import { ResumeExperienceService } from '../../core/services/resume-experience.service';

describe('Resumes', () => {
  let component: Resumes;
  let fixture: ComponentFixture<Resumes>;

  let resumeServiceSpy: jasmine.SpyObj<ResumeService>;
  let resumeExperienceServiceSpy: jasmine.SpyObj<ResumeExperienceService>;

  beforeEach(async () => {
    resumeServiceSpy = jasmine.createSpyObj<ResumeService>(
      'ResumeService',
      ['getAll', 'create', 'update', 'delete']
    );

    resumeExperienceServiceSpy =
      jasmine.createSpyObj<ResumeExperienceService>(
        'ResumeExperienceService',
        ['getAll', 'create', 'update', 'delete']
      );

    resumeServiceSpy.getAll.and.returnValue(of([]));
    resumeExperienceServiceSpy.getAll.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [Resumes],

      providers: [
        // Required by indirect dependencies such as:
        // Navbar -> AuthService -> HttpClient
        provideHttpClient(),

        // Required by RouterLink in Resumes/Navbar
        provideRouter([]),

        {
          provide: ResumeService,
          useValue: resumeServiceSpy
        },

        {
          provide: ResumeExperienceService,
          useValue: resumeExperienceServiceSpy
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Resumes);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load resumes on initialization', () => {
    expect(resumeServiceSpy.getAll).toHaveBeenCalled();
  });
});