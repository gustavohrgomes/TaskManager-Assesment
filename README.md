# BallastLane Task Manager

A .NET 10 Clean Architecture task manager with Angular SPA, built TDD-first for the BallastLane technical interview.

## User Story

As a busy professional, I need a simple way to track my tasks with statuses and due dates. I want to register an account, log in, and manage my tasks through a clean web interface. I should be able to create tasks, update their status as I make progress, and delete them when done. My tasks are private to me: nobody else can see or modify them.

## Prerequisites

- Docker Desktop (for PostgreSQL via Aspire)
- .NET 10 SDK
- Node.js 22 LTS
- npm (comes with Node.js)

## Quick Start

```bash
git clone <repo-url>
cd TaskManager-Assesment
dotnet run --project TaskManagement.AppHost
```

The Aspire AppHost launches PostgreSQL (with seed data), the API, and the Angular SPA. The Aspire dashboard opens automatically.

## Running Tests

```bash
dotnet test
```

Test projects:
- **Domain.Tests** - Entity invariants, value objects, status transitions
- **Application.Tests** - Use case handlers with mocked gateways
- **Infrastructure.Tests** - Password hasher and JWT issuer unit tests
- **ArchitectureTests** - Clean Architecture layer boundary verification via NetArchTest

## Demo Credentials

```
Email: demo@ballastlane.com
Password: Demo123!
```

The seed script creates this user with 5 sample tasks.

## Demo Walkthrough

1. Run `dotnet run --project TaskManagement.AppHost`
2. Open the Aspire dashboard (URL shown in terminal output)
3. Click the Angular SPA endpoint (or navigate to the port shown)
4. Log in with the demo credentials above
5. View the 5 seeded tasks in the card grid
6. Create a new task using the "+ New Task" button
7. Edit a task using the pencil icon
8. Change a task's status using the Start/Complete buttons
9. Delete a task using the trash icon
10. Open Scalar API Reference from the Aspire dashboard (or navigate to `/scalar` on the API URL)
11. Run `dotnet test` in a separate terminal to see all tests pass
12. Click Logout in the nav bar

## Architecture

```
Domain          (zero dependencies)
  ^
Application     (depends on Domain only)
  ^
Infrastructure  (depends on Application; owns Npgsql, JWT, password hashing)
  ^
API             (composition root; wires everything via DI)
```

The dependency rule flows inward. Domain has no package or project references. Application defines gateway interfaces (`IUserRepository`, `ITaskRepository`, `IPasswordHasher`, `IJwtTokenIssuer`). Infrastructure implements them with raw Npgsql SQL and ASP.NET Core Identity's `PasswordHasher<T>`. The API is the only composition root.

Architecture tests in `tests/BallastLane.TaskManager.ArchitectureTests/` verify these boundaries at build time using NetArchTest.Rules. The four tests (`Domain_should_have_no_external_dependencies`, `Application_should_not_reference_Npgsql`, `Controllers_should_not_depend_on_Npgsql`, `Banned_packages_are_absent`) will fail the build if any layer violation is introduced.

## Security Notes

| Area | Approach | Notes |
|------|----------|-------|
| Password Hashing | PBKDF2 via `PasswordHasher<T>` | Per-user salt, industry-standard iteration count |
| JWT Validation | All flags true: ValidateIssuer, ValidateAudience, ValidateLifetime, ValidateIssuerSigningKey | HS256 only, 30-second clock skew, signing key from Aspire parameter (never in source) |
| Token Storage | localStorage in the Angular SPA | Tradeoff: simpler implementation, XSS risk accepted for this demo scope. Production would use httpOnly cookies with CSRF protection. |
| Timing-Safe Login | Dummy hash comparison on missing user | Prevents user enumeration via response time differential |

## Scope Decisions

- **No refresh tokens:** Single JWT with expiry is sufficient for the demo. Production would add refresh token rotation.
- **No integration tests:** Dropped from Phase 3 for time budget. Unit tests cover Domain, Application, Infrastructure; architecture tests verify layering.
- **localStorage for JWT:** Accepted XSS tradeoff documented in Security Notes. Production would use httpOnly cookies.
- **Single SQL migration file:** `db/001_init.sql` creates the schema. Production would use a migration tool like dbup or Flyway.
- **No RBAC / multi-role authorization:** Single "authenticated user" role covers the spec requirements.

## Tech Stack

- .NET 10, ASP.NET Core (controllers)
- PostgreSQL (raw Npgsql, parameterized SQL)
- Angular 20 LTS (Material 20)
- .NET Aspire (AppHost, ServiceDefaults)
- xUnit v3
- FluentValidation 12
- Scalar (OpenAPI UI)

## GenAI Tools

This project was built using Claude Code with structured planning and execution workflows. For the GenAI artifact required by the interview spec, see [GENAI-ARTIFACT.md](GENAI-ARTIFACT.md).

## License

This project was created as a technical interview exercise for BallastLane.
