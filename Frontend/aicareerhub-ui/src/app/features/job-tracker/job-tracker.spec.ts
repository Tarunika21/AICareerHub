import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobTracker } from './job-tracker';

describe('JobTracker', () => {
  let component: JobTracker;
  let fixture: ComponentFixture<JobTracker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobTracker]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobTracker);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
