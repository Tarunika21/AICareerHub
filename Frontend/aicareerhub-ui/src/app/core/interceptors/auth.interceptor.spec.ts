import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { authInterceptor } from './auth.interceptor';
import { environment } from '../../../environments/environment';

describe('authInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
    localStorage.clear();
  });

  it('should attach bearer token to AICareerHub API requests', () => {
    const token = 'test-jwt-token';
    localStorage.setItem('token', token);

    httpClient.get(`${environment.apiUrl}/resumes`).subscribe();

    const request = httpTestingController.expectOne(`${environment.apiUrl}/resumes`);

    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);

    request.flush({});
  });

  it('should not attach authorization header when token does not exist', () => {
    httpClient.get(`${environment.apiUrl}/resumes`).subscribe();

    const request = httpTestingController.expectOne(`${environment.apiUrl}/resumes`);

    expect(request.request.headers.has('Authorization')).toBeFalse();

    request.flush({});
  });

  it('should not attach token to external API requests', () => {
    localStorage.setItem('token', 'test-jwt-token');

    const externalUrl = 'https://example.com/jobs';

    httpClient.get(externalUrl).subscribe();

    const request = httpTestingController.expectOne(externalUrl);

    expect(request.request.headers.has('Authorization')).toBeFalse();

    request.flush({});
  });
});
