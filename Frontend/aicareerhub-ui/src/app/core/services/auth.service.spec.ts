import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';
import { LoginRequest } from '../models/login-request';
import { RegisterRequest } from '../models/register-request';
import { AuthResponse } from '../models/auth-response';
import { User } from '../models/user';

describe('AuthService', () => {
  let service: AuthService;
  let httpTestingController: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/auth`;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(AuthService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login and store token and user in localStorage', () => {
    const request: LoginRequest = {
      email: 'test@example.com',
      password: 'Password123!',
    };

    const user = {
      id: 'user-1',
      email: 'test@example.com',
    } as User;

    const response: AuthResponse = {
      token: 'test-token',
      expiresAt: '2026-09-26T19:00:00Z',
      user,
    };

    service.login(request).subscribe((result) => {
      expect(result).toEqual(response);
      expect(localStorage.getItem('token')).toBe('test-token');
      expect(JSON.parse(localStorage.getItem('user')!)).toEqual(user);
    });

    const httpRequest = httpTestingController.expectOne(`${apiUrl}/login`);

    expect(httpRequest.request.method).toBe('POST');
    expect(httpRequest.request.body).toEqual(request);

    httpRequest.flush(response);
  });

  it('should register a new user', () => {
    const request: RegisterRequest = {
      email: 'newuser@example.com',
      password: 'Password123!',
    } as RegisterRequest;

    const response = {
      id: 'user-1',
      email: 'newuser@example.com',
    } as User;

    service.register(request).subscribe((result) => {
      expect(result).toEqual(response);
    });

    const httpRequest = httpTestingController.expectOne(`${apiUrl}/register`);

    expect(httpRequest.request.method).toBe('POST');
    expect(httpRequest.request.body).toEqual(request);

    httpRequest.flush(response);
  });

  it('should return token from localStorage', () => {
    localStorage.setItem('token', 'stored-token');

    expect(service.getToken()).toBe('stored-token');
  });

  it('should return null when token does not exist', () => {
    expect(service.getToken()).toBeNull();
  });

  it('should return true when user is logged in', () => {
    localStorage.setItem('token', 'stored-token');

    expect(service.isLoggedIn()).toBeTrue();
  });

  it('should return false when user is not logged in', () => {
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('should remove token and user when logging out', () => {
    localStorage.setItem('token', 'stored-token');
    localStorage.setItem('user', JSON.stringify({ id: 'user-1' }));

    service.logout();

    expect(localStorage.getItem('token')).toBeNull();
    expect(localStorage.getItem('user')).toBeNull();
  });
});
