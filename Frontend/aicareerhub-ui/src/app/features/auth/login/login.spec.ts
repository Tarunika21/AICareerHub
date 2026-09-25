import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { of } from 'rxjs';

import { Login } from './login';
import { AuthService } from '../../../core/services/auth.service';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let router: Router;

  const authServiceMock = {
    login: jasmine.createSpy('login').and.returnValue(of({})),
  };

  beforeEach(async () => {
    authServiceMock.login.calls.reset();
    await TestBed.configureTestingModule({
      imports: [Login],

      providers: [
        provideRouter([]),

        {
          provide: AuthService,
          useValue: authServiceMock,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;

    router = TestBed.inject(Router);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not submit when form is invalid', () => {
    component.loginForm.setValue({
      email: '',
      password: '',
    });

    component.onSubmit();

    expect(authServiceMock.login).not.toHaveBeenCalled();
  });

  it('should call login when form is valid', () => {
    component.loginForm.setValue({
      email: 'test@example.com',
      password: 'Password123!',
    });

    component.onSubmit();

    expect(authServiceMock.login).toHaveBeenCalled();
  });

  it('should navigate to dashboard after successful login', () => {
    spyOn(router, 'navigate');

    component.loginForm.setValue({
      email: 'test@example.com',
      password: 'Password123!',
    });

    component.onSubmit();

    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});