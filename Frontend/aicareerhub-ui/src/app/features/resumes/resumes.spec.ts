import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { Resumes } from './resumes';

describe('Resumes', () => {
  let component: Resumes;
  let fixture: ComponentFixture<Resumes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Resumes],

      providers: [
        provideHttpClient(),
        provideRouter([]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Resumes);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});