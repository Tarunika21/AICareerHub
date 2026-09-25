import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Dashboard } from './features/dashboard/dashboard';
import { JobTracker } from './features/job-tracker/job-tracker';
import { CareerProfile } from './features/career-profile/career-profile';
import { Resumes } from './features/resumes/resumes';
import { ResumeBuilder } from './features/resume-builder/resume-builder';
import { ResumePreview } from './features/resume-preview/resume-preview';

import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },

  {
    path: 'login',
    component: Login,
  },

  {
    path: 'register',
    component: Register,
  },

  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard],
  },

  {
    path: 'job-tracker',
    component: JobTracker,
    canActivate: [authGuard],
  },

  {
    path: 'career-profile',
    component: CareerProfile,
    canActivate: [authGuard],
  },

  {
    path: 'resumes/new',
    component: ResumeBuilder,
    canActivate: [authGuard],
  },

  {
    path: 'resumes/:id/edit',
    component: ResumeBuilder,
    canActivate: [authGuard],
  },

  {
    path: 'resumes/:id/preview',
    component: ResumePreview,
    canActivate: [authGuard],
  },

  {
    path: 'resumes',
    component: Resumes,
    canActivate: [authGuard],
  },

  {
    path: '**',
    redirectTo: 'login',
  },
];