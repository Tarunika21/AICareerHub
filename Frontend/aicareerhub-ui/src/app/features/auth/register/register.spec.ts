import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { Register } from './register';
import { AuthService } from '../../../core/services/auth.service';

describe('Register', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;
  let router: Router;

  const authServiceMock = {
    register: jasmine.createSpy('register').and.returnValue(of({})),
  };

  beforeEach(async () => {
    authServiceMock.register.calls.reset();
    authServiceMock.register.and.returnValue(of({}));

    await TestBed.configureTestingModule({
      imports: [Register],

      providers: [
        provideRouter([]),

        {
          provide: AuthService,
          useValue: authServiceMock,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;

    router = TestBed.inject(Router);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not submit when form is invalid', () => {
    component.registerForm.setValue({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
    });

    component.onSubmit();

    expect(authServiceMock.register).not.toHaveBeenCalled();
  });

  it('should call register when form is valid', () => {
    component.registerForm.setValue({
      firstName: 'Test',
      lastName: 'User',
      email: 'test@example.com',
      password: 'Password123!',
    });

    component.onSubmit();

    expect(authServiceMock.register).toHaveBeenCalled();
  });

  it('should navigate to login after successful registration', () => {
    spyOn(router, 'navigate');

    component.registerForm.setValue({
      firstName: 'Test',
      lastName: 'User',
      email: 'test@example.com',
      password: 'Password123!',
    });

    component.onSubmit();

    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should show account exists message for 409 error', () => {
    authServiceMock.register.and.returnValue(
      throwError(() => ({
        status: 409,
      })),
    );

    component.registerForm.setValue({
      firstName: 'Test',
      lastName: 'User',
      email: 'existing@example.com',
      password: 'Password123!',
    });

    component.onSubmit();

    expect(component.errorMessage).toBe(
      'An account with this email already exists.',
    );

    expect(component.isLoading).toBeFalse();
  });

  it('should show generic error for other registration errors', () => {
    authServiceMock.register.and.returnValue(
      throwError(() => ({
        status: 500,
      })),
    );

    component.registerForm.setValue({
      firstName: 'Test',
      lastName: 'User',
      email: 'test@example.com',
      password: 'Password123!',
    });

    component.onSubmit();

    expect(component.errorMessage).toBe(
      'Unable to create account. Please try again.',
    );

    expect(component.isLoading).toBeFalse();
  });
});