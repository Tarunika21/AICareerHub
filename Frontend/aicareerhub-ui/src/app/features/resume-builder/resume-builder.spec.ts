import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { ResumeBuilder } from './resume-builder';

describe('ResumeBuilder', () => {
  let component: ResumeBuilder;
  let fixture: ComponentFixture<ResumeBuilder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResumeBuilder],

      providers: [
        provideRouter([]),
        provideHttpClient(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ResumeBuilder);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});