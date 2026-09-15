import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Dashboard } from './features/dashboard/dashboard';
import { JobTracker } from './features/job-tracker/job-tracker';
import { authGuard } from './core/guards/auth.guard';
import { CareerProfile } from './features/career-profile/career-profile';
export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: Login },
  { path: 'register', component: Register },

  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard]
  },
  {
    path: 'job-tracker',
    component: JobTracker,
    canActivate: [authGuard]
  },
  {
    path: 'career-profile',
    component: CareerProfile,
    canActivate: [authGuard]
  },

  { path: '**', redirectTo: 'login' }
];