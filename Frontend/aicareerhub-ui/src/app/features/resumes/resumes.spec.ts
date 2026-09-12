import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Resumes } from './resumes';

describe('Resumes', () => {
  let component: Resumes;
  let fixture: ComponentFixture<Resumes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Resumes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Resumes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
