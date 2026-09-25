import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', [
      'isLoggedIn',
    ]);

    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        {
          provide: AuthService,
          useValue: authServiceSpy,
        },
      ],
    });

    router = TestBed.inject(Router);
  });

  it('should allow navigation when user is logged in', () => {
    authServiceSpy.isLoggedIn.and.returnValue(true);

    const result = TestBed.runInInjectionContext(() => authGuard(null!, null!));

    expect(result).toBeTrue();
    expect(authServiceSpy.isLoggedIn).toHaveBeenCalled();
  });

  it('should redirect to login when user is not logged in', () => {
    authServiceSpy.isLoggedIn.and.returnValue(false);

    const result = TestBed.runInInjectionContext(() => authGuard(null!, null!));

    const expectedUrlTree = router.createUrlTree(['/login']);

    expect(result).toEqual(expectedUrlTree);
    expect(router.serializeUrl(result as any)).toBe('/login');
    expect(authServiceSpy.isLoggedIn).toHaveBeenCalled();
  });
});