import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { CareerProfile } from './career-profile';

describe('CareerProfile', () => {
  let component: CareerProfile;
  let fixture: ComponentFixture<CareerProfile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CareerProfile],

      providers: [
        provideHttpClient(),
        provideRouter([]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CareerProfile);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});