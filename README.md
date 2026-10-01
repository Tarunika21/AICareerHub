# AICareerHub

A full-stack career management platform for creating and managing resumes, maintaining career profiles, tracking job applications, and providing AI-assisted resume suggestions.

AICareerHub was built as an end-to-end engineering project covering application architecture, authentication, relational database design, frontend development, automated testing, AWS deployment, and production troubleshooting.

## Features

### Authentication

- User registration and login
- Secure password hashing
- JWT-based authentication
- Protected frontend routes
- Protected backend APIs

### Career Profile

Users can create and update their career profile, including information used by other features such as the target role.

### Resume Management

Users can:

- Create multiple resumes
- Update resume details
- Add professional summaries
- Maintain skills
- Add work experience
- Add education
- Add projects
- Edit and delete resume sections
- Preview resume information

### AI-Assisted Resume Features

The application contains an AI abstraction that supports:

- Professional-summary improvement
- Experience-description improvement
- Resume suggestions based on job descriptions

The project is designed so AI functionality can work with a mock/offline provider without requiring a paid external AI API.

### Job Application Tracker

Users can:

- Add job applications
- Update existing applications
- Delete applications
- Track application status
- Search applications
- Filter applications by status
- Maintain application notes and job details

---

# Technology Stack

## Frontend

- Angular 20
- TypeScript
- Reactive Forms
- Angular HttpClient
- RxJS
- Route Guards
- Jasmine/Karma

## Backend

- ASP.NET Core Web API
- .NET 10
- C#
- Entity Framework Core
- Repository pattern
- Service layer
- DTO-based API contracts
- Dependency Injection
- Global exception handling
- ProblemDetails
- Swagger / OpenAPI

## Authentication

- JWT Bearer Authentication
- ASP.NET Core PasswordHasher

## Database

- PostgreSQL 15
- Entity Framework Core
- Npgsql
- EF Core Migrations

## Testing

- xUnit
- ASP.NET Core integration-test infrastructure
- Angular Jasmine/Karma tests

Latest recorded backend test result:

```text
Total: 51
Passed: 51
Failed: 0
Skipped: 0
```

## Cloud / Deployment

- AWS Elastic Beanstalk
- Amazon EC2
- Amazon S3
- Amazon CloudFront — planned
- Amazon Linux 2023

---

# Architecture

```text
                    Browser
                       |
                       |
                  Angular SPA
                       |
                  HTTP / JSON
                       |
                       v
              ASP.NET Core API
                       |
          +------------+------------+
          |            |            |
     Controllers    Services    Authentication
          |
     Repositories
          |
     Entity Framework Core
          |
        Npgsql
          |
      PostgreSQL
```

The backend follows a layered architecture:

```text
Controller
    |
    v
Service
    |
    v
Repository
    |
    v
Entity Framework Core
    |
    v
PostgreSQL
```

This separation keeps HTTP handling, business logic, and persistence responsibilities independent.

---

# Production Architecture

The intended production architecture is:

```text
                         Browser
                            |
                          HTTPS
                            |
                            v
                       CloudFront
                      /          \
                     /            \
                    v              v
             Angular / S3      /api/*
                                   |
                                   v
                         Elastic Beanstalk
                                   |
                                   v
                          ASP.NET Core API
                                   |
                                   v
                              PostgreSQL
```

The Angular production configuration uses:

```text
/api
```

instead of directly calling the HTTP Elastic Beanstalk hostname.

This allows CloudFront to eventually provide a single HTTPS browser origin for both the frontend and API.

---

# Backend Structure

The backend follows a layered design consisting primarily of:

```text
Controllers
DTOs
Models
Services
Repositories
Data / DbContext
Exception Handling
Authentication
Configuration
Tests
```

## Controllers

Controllers expose HTTP endpoints and delegate business operations to services.

Major areas include:

- Authentication
- Users
- Career Profiles
- Resumes
- Resume Experiences
- Resume Education
- Resume Projects
- Job Applications
- AI-assisted resume operations

## Services

Services contain application and business logic.

Examples include:

- Authentication service
- User service
- Career profile service
- Resume service
- Resume experience service
- Resume education service
- Resume project service
- AI service abstraction

## Repositories

Repositories encapsulate database operations and keep persistence concerns separate from application logic.

## DTOs

DTOs are used for API request and response contracts instead of exposing database entities directly.

---

# Database Design

The application uses PostgreSQL with Entity Framework Core.

Major application data includes:

```text
Users
CareerProfiles
Resumes
ResumeExperiences
ResumeEducations
ResumeProjects
JobApplications
```

The database schema is managed through EF Core migrations.

Important schema changes during development included:

- Initial database creation
- Unique email constraint
- Password-hash support
- Career profile
- Resume management
- Resume-related sections
- Job applications

---

# Authentication Flow

## Registration

```text
Angular
   |
   v
Register API
   |
   v
Auth Service
   |
   +--> Validate user
   |
   +--> Hash password
   |
   v
User Repository
   |
   v
PostgreSQL
```

Passwords are never intentionally stored as plain text.

## Login

```text
Login Request
     |
     v
Find User
     |
     v
Verify Password
     |
     v
Generate JWT
     |
     v
Return Token
```

The Angular application sends the token with authenticated API requests.

---

# Error Handling

The backend uses centralized exception handling.

Examples include:

```text
ConflictException
        -> HTTP 409

UnauthorizedAccessException
        -> HTTP 401

Unexpected exception
        -> HTTP 500
```

ProblemDetails-style responses are used to provide consistent API errors.

---

# API Documentation

Swagger/OpenAPI is enabled for API discovery and testing.

During AWS deployment, Swagger was also used to confirm that the Elastic Beanstalk-hosted API was functioning correctly.

A request to the root Elastic Beanstalk URL may return:

```text
404 Not Found
```

because the API does not define a root webpage.

The Swagger endpoint is the appropriate API verification route.

---

# Testing

Testing was introduced across multiple layers.

## Backend

Backend tests cover service/application behavior and integration-style application hosting.

Latest recorded result:

```text
51 tests passed
0 tests failed
0 tests skipped
```

## Frontend

Angular component tests use Jasmine/Karma and service spies to test UI logic without depending on the production backend.

Testing areas include:

- Component initialization
- Data loading
- Filtering
- Form behavior
- Create/update/delete operations
- Error handling

---

# AWS Deployment

The backend has been deployed using AWS Elastic Beanstalk.

The deployment environment uses:

```text
.NET 10
Amazon Linux 2023
Elastic Beanstalk
```

Production application configuration is supplied through environment properties rather than storing production secrets in the repository.

Examples of configuration keys include:

```text
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
Jwt__ExpiryMinutes
AI__Provider
```

Actual secret values are intentionally not stored in this repository.

---

# AWS Deployment Troubleshooting

The deployment process included several real production troubleshooting scenarios.

## EF Core Migration Bundle

A Linux-targeted migration bundle was initially attempted from Windows.

The first problem was:

```text
NETSDK1047
```

because Linux runtime assets had not been restored.

A runtime-specific restore was performed.

Later, after transferring the generated migration bundle to the Amazon Linux instance, the Linux `file` command identified the artifact as:

```text
PE32+ executable (console) x86-64, for MS Windows
```

Instead of continuing with the unreliable cross-platform bundle approach, the deployment strategy was changed.

The repository was cloned directly onto the Linux instance, the .NET SDK and EF CLI were installed, and migrations were executed from the source project.

## EF Design-Time Configuration

Running:

```text
dotnet ef database update
```

initially failed because application startup required JWT configuration.

The required database and JWT configuration was supplied to the Linux environment before running EF.

The final migration result was:

```text
No migrations were applied.
The database is already up to date.
Done.
```

## Elastic Beanstalk Health

During deployment the Elastic Beanstalk environment entered:

```text
Severe
```

health.

After resolving application configuration and database/runtime issues, the environment recovered to:

```text
OK
```

Swagger was then successfully used to verify the deployed API.

---

# HTTPS and CloudFront

The deployed Elastic Beanstalk API currently uses an HTTP origin.

An HTTPS frontend directly calling an HTTP API would result in browser mixed-content restrictions.

For this reason the Angular production API configuration was changed to:

```text
/api
```

The planned CloudFront configuration is:

```text
Default behavior
    -> Angular frontend / S3

/api/*
    -> Elastic Beanstalk API
```

This allows the browser to communicate through a single HTTPS CloudFront origin.

## Current Deployment Status

CloudFront configuration is currently pending because of an AWS account-verification/support issue.

The application itself has already reached the following checkpoint:

```text
Backend deployed              DONE
Database migrations           DONE
JWT configuration             DONE
Swagger verification          DONE
Elastic Beanstalk health      OK
Backend tests                 51 / 51 PASSING
Angular production /api       DONE
CloudFront                    PENDING
HTTPS end-to-end testing      PENDING
```

CloudFront and final HTTPS testing will be completed after the AWS account restriction is resolved.

---

# Local Development

## Prerequisites

Install:

- .NET 10 SDK
- Node.js
- npm
- Angular CLI
- PostgreSQL
- Git
- Visual Studio with ASP.NET/web-development support

## Verify Installation

```bash
dotnet --list-sdks
node --version
npm --version
ng version
git --version
psql --version
```

## Backend

Navigate to the API project and restore/build it.

```bash
dotnet restore
dotnet build
```

Configure the required development settings, including the PostgreSQL connection string and JWT configuration.

Apply migrations:

```bash
dotnet ef database update
```

Run the API through Visual Studio or:

```bash
dotnet run
```

## Frontend

Navigate to the Angular application.

```bash
npm install
ng serve
```

The development API configuration points to the local ASP.NET Core API.

---

# Running Tests

## Backend

From the appropriate backend test project or solution:

```bash
dotnet test
```

## Frontend

From the Angular project:

```bash
ng test
```

---

# Security

This repository should never contain:

- Production database passwords
- JWT signing secrets
- AWS access keys
- Private connection strings
- Other production credentials

A useful repository check is:

```bash
git grep -n -i -E "password=|Jwt.*Key|DefaultConnection|secret"
```

Every match should be reviewed before publishing the repository.

---

# Documentation

Detailed project documentation is maintained separately under:

```text
docs/
```

The master engineering document contains:

- Environment setup
- Backend fundamentals
- Architecture
- Database design
- Authentication
- Resume implementation
- Angular implementation
- Testing
- Git/GitHub workflow
- AWS deployment
- EC2 troubleshooting
- EF Core production migrations
- Elastic Beanstalk troubleshooting
- HTTPS/CloudFront design
- Challenges and resolutions
- Commands
- Production checklist
- Interview preparation notes

---

# Project Status

AICareerHub is currently in the final production-deployment stage.

The core application, database, automated backend tests, and AWS-hosted backend are operational.

Remaining production work:

```text
AWS account verification
        |
        v
CloudFront
        |
        v
HTTPS routing
        |
        v
Final Angular deployment
        |
        v
Production E2E testing
        |
        v
Portfolio release
```

---

# Author

**Tarunika V**

Software Engineer

Full-stack development with ASP.NET Core, Angular, PostgreSQL and cloud technologies.