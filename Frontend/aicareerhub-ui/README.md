# AICareerHub

AICareerHub is a full-stack career management web application built to help users manage their career profile, resume information, experience, education, projects, and job-related activities from a centralized platform.

The project was built as an end-to-end full-stack application with a production-style architecture, authentication, relational database integration, automated testing, AWS deployment, and CI/CD using GitHub Actions.

## Tech Stack

### Frontend
- Angular 20
- TypeScript
- HTML
- CSS
- RxJS
- Angular Router
- HTTP Client

### Backend
- ASP.NET Core Web API
- .NET 10
- C#
- Entity Framework Core
- Repository and Service architecture
- DTO-based API communication
- Global exception handling
- Swagger / OpenAPI

### Database
- PostgreSQL
- Entity Framework Core migrations

### Authentication & Security
- JWT authentication
- ASP.NET Core authentication middleware
- Password hashing using ASP.NET Core Identity `PasswordHasher`
- Protected API endpoints
- CORS configuration

### Testing
- Backend unit tests
- Automated backend build and test execution through GitHub Actions
- Angular production build validation through CI

### Cloud & DevOps
- AWS Elastic Beanstalk — ASP.NET Core API hosting
- Amazon S3 — Angular production build hosting
- Amazon CloudFront — planned HTTPS/CDN layer
- GitHub Actions — CI/CD
- GitHub OIDC authentication with AWS
- AWS IAM roles and policies

---

## Architecture

The application follows a separated frontend/backend architecture:

```text
User
  |
  v
Angular Frontend
  |
  | HTTP / REST API
  v
ASP.NET Core Web API
  |
  v
Service Layer
  |
  v
Repository Layer
  |
  v
Entity Framework Core
  |
  v
PostgreSQL
```

Production deployment architecture:

```text
GitHub
   |
   v
GitHub Actions
   |
   +----------------------+
   |                      |
   v                      v
Backend CI/CD         Frontend CI/CD
   |                      |
   v                      v
Elastic Beanstalk      Amazon S3
                              |
                              v
                         CloudFront
```

CloudFront integration is intended to provide HTTPS and CDN delivery for the Angular frontend while keeping the S3 bucket private.

---

## Main Features

- User registration
- User login
- JWT-based authentication
- Career profile management
- Resume management
- Resume experience management
- Resume education management
- Resume project management
- Centralized exception handling
- API validation
- PostgreSQL persistence
- Angular frontend integration with the backend API
- Automated CI/CD deployment

---

## Project Structure

```text
AICareerHub/
│
├── Backend/
│   └── AICareerHub.API/
│       ├── AICareerHub.API/
│       └── AICareerHub.API.Tests/
│
├── Frontend/
│   └── aicareerhub-ui/
│       ├── src/
│       ├── public/
│       ├── angular.json
│       └── package.json
│
├── .github/
│   └── workflows/
│       └── ci.yml
│
└── README.md
```

---

## Backend Setup

### Prerequisites

Install:

- .NET SDK 10
- PostgreSQL
- Git

Verify:

```bash
dotnet --version
psql --version
git --version
```

### Restore Dependencies

```bash
dotnet restore
```

### Apply Database Migrations

Configure the PostgreSQL connection string before running migrations.

Then run:

```bash
dotnet ef database update
```

### Run the API

```bash
dotnet run
```

During local development, the API is configured to run through the ASP.NET Core development environment.

Swagger can be used to inspect and test the available API endpoints.

---

## Frontend Setup

### Prerequisites

- Node.js 22
- npm
- Angular CLI 20

Verify:

```bash
node --version
npm --version
ng version
```

### Install Dependencies

Navigate to:

```text
Frontend/aicareerhub-ui
```

Then run:

```bash
npm install
```

### Run Locally

```bash
ng serve
```

Open:

```text
http://localhost:4200
```

The development environment communicates with the locally running backend API.

---

## Angular Environments

Development configuration:

```text
src/environments/environment.ts
```

Production configuration:

```text
src/environments/environment.prod.ts
```

Angular replaces the development environment with the production environment during a production build.

This is configured through `fileReplacements` in `angular.json`.

---

## Production Build

To create the Angular production build:

```bash
npm run build
```

The generated browser application is deployed to Amazon S3 by the CI/CD pipeline.

---

## CI/CD Pipeline

GitHub Actions automatically runs when code is pushed to the `main` branch.

The pipeline performs four major jobs:

```text
Push to main
      |
      +--> Backend Build & Test
      |
      +--> Frontend Build
                |
                v
      -----------------------
      |                     |
      v                     v
Deploy Backend        Deploy Frontend
Elastic Beanstalk       Amazon S3
```

### Backend Pipeline

The backend pipeline:

1. Checks out the repository
2. Installs .NET 10
3. Restores NuGet dependencies
4. Builds the API
5. Runs backend tests
6. Publishes the application
7. Authenticates with AWS using GitHub OIDC
8. Creates an Elastic Beanstalk application version
9. Deploys the new version
10. Waits for deployment completion
11. Verifies Elastic Beanstalk environment health

### Frontend Pipeline

The frontend pipeline:

1. Checks out the repository
2. Installs Node.js 22
3. Installs dependencies using `npm ci`
4. Builds the Angular production application
5. Authenticates with AWS using GitHub OIDC
6. Synchronizes the generated browser files with the S3 frontend bucket

---

## AWS Authentication

The CI/CD pipeline uses GitHub Actions OIDC authentication rather than storing permanent AWS access keys in GitHub.

```text
GitHub Actions
      |
      | OIDC token
      v
AWS IAM Role
      |
      +--> Elastic Beanstalk
      |
      +--> S3
```

This avoids storing long-lived AWS credentials inside the repository.

---

## AWS Deployment

### Backend

The ASP.NET Core API is deployed using AWS Elastic Beanstalk.

Elastic Beanstalk manages the underlying compute environment used to run the API.

### Frontend

The Angular production build is uploaded to a private Amazon S3 bucket.

The S3 bucket is not intended to be exposed directly to the internet.

### CloudFront

Amazon CloudFront is planned as the public entry point for the frontend.

The intended architecture is:

```text
Browser
   |
 HTTPS
   |
   v
CloudFront
   |
   v
Private S3 Bucket
```

CloudFront setup is currently dependent on AWS account verification for creation of new CloudFront resources.

---

## CI/CD Status

The current GitHub Actions pipeline successfully performs:

- Backend build
- Backend tests
- Frontend build
- Backend deployment to AWS Elastic Beanstalk
- Frontend deployment to Amazon S3

CloudFront configuration and final HTTPS frontend-to-backend integration remain part of the deployment completion process.

---

## Future Improvements

Potential enhancements include:

- Complete CloudFront HTTPS configuration
- Custom domain configuration
- Improved frontend automated testing
- End-to-end testing
- Enhanced application monitoring
- Additional career management features
- AI-assisted career