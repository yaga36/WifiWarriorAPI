# WifiWarrior API

WifiWarrior is a travel-focused Wi-Fi sharing API for storing and finding public venue Wi-Fi information.

The idea came from travelling abroad and needing internet access in cafes, restaurants, bars, and other public venues when I did not have a local SIM card. The goal is to let travellers find somewhere nearby to get online quickly, and allow other users to contribute useful Wi-Fi details as they discover them.

This repository contains the ASP.NET Core backend API.

## Project Status

WifiWarrior is a work in progress.

The original version was built several years ago when I was earlier in my .NET career. This version is a modernisation of that idea, focused on improving the backend architecture, security, testing, and CI so the project better reflects how I would approach a .NET API today.

It is not a finished production application yet. The current API provides the foundation for authentication, venue data, Wi-Fi credential handling, role-based authorisation, DTO boundaries, automated tests, and CI. The next stage would be building out the moderation and administration workflows needed for a real community-driven product.

## Core Features

- User registration and login with JWT bearer authentication.
- Role-based authorisation for users, moderators, and administrators.
- Venue management for cafes, restaurants, bars, and other public spaces.
- Address, connection type, connection information, and Wi-Fi detail endpoints.
- Full venue setup endpoint for creating a venue, address, connection, and Wi-Fi details together.
- Wi-Fi credentials encrypted at rest.
- DTO-based API contracts instead of exposing EF Core entities directly.
- Centralised exception handling with `ProblemDetails`.
- Unit and integration test coverage for auth and domain flows.
- GitHub Actions CI for restore, build, and test.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL / Npgsql
- ASP.NET Core Identity
- JWT bearer authentication
- ASP.NET Core Data Protection
- Swagger / OpenAPI
- xUnit
- WebApplicationFactory
- EF Core InMemory provider for integration tests
- GitHub Actions

## Architecture

The backend follows a simple layered structure:

```text
HTTP Request
    |
Controller
    |
Service
    |
EF Core DbContext
    |
PostgreSQL
```

Controllers are responsible for HTTP concerns such as routing, status codes, authorisation, and request/response models.
Services contain application logic and coordinate persistence through EF Core.
DTOs are used at the API boundary so EF Core entities are not exposed directly to clients.
Authentication uses ASP.NET Core Identity for user management and JWT bearer tokens for API access.
Authorisation is policy-based, with roles such as:

- `User`
- `Moderator`
- `Administrator`

## Security Notes

- User passwords are handled by ASP.NET Core Identity.
- Wi-Fi passwords are stored encrypted rather than as plaintext.
- JWT settings are loaded from configuration, user secrets, or environment variables.
- Development settings are ignored from source control.
- Role-based policies protect write and administrative operations.
- Centralised exception handling prevents raw exceptions being returned to clients.

## Running Locally

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Rider, Visual Studio, or VS Code

### Configuration

Set local configuration using user secrets or environment variables.

Example using user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:PostgresDbConnection" "Host=localhost;Database=wifiwarrior;Username=postgres;Password=your-password" --project WifiWarriorAPI
dotnet user-secrets set "JWT:Key" "replace-with-a-long-development-secret" --project WifiWarriorAPI
dotnet user-secrets set "JWT:Issuer" "WifiWarriorAPI" --project WifiWarriorAPI
dotnet user-secrets set "JWT:Audience" "WifiWarriorClient" --project WifiWarriorAPI
dotnet user-secrets set "JWT:LifeTime" "15" --project WifiWarriorAPI
dotnet user-secrets set "Cors:AllowedOrigins:0" "http://localhost:3000" --project WifiWarriorAPI
```

For production, these values should be supplied through environment variables or the hosting provider's secret/configuration system.

## Database

Apply EF Core migrations:

```bash
dotnet ef database update --project WifiWarriorAPI
```

## Run the API

```bash
dotnet run --project WifiWarriorAPI
```

Swagger is available in development mode.

## Testing

Run the full backend test suite:

```bash
dotnet test WifiWarriorAPI.sln --configuration Release
```

The test suite includes:

- Unit tests for service logic.
- Integration tests using `WebApplicationFactory`.
- Authentication and authorisation tests.
- CRUD endpoint tests.
- `ProblemDetails`/error handling coverage.
- Tests around encrypted Wi-Fi credential handling.

## CI

GitHub Actions runs the backend CI pipeline on push and pull request to `main`.

The pipeline runs:

```text
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

This gives confidence that the API builds and the automated test suite passes in a clean environment.

## Roadmap

Planned improvements include:

- Submission workflow for new venues and Wi-Fi updates.
- Moderator review queue before public changes go live.
- Administrator account management.
- Password reset and email confirmation flows.
- User deletion, locking, and account recovery.
- Audit logs for sensitive actions.
- Venue search by location, distance, category, and connection type.
- User confirmation system for whether Wi-Fi details still work.
- Expiry/review dates for old Wi-Fi credentials.
- Duplicate venue detection and merge tooling.
- More granular permissions.
- API versioning.
- Improved OpenAPI documentation.
- Health checks and observability.
- Production deployment pipeline.

## Frontend

The frontend is maintained separately as a React TypeScript application.

Frontend repository:

```text
https://github.com/yaga36/WifiWarriorReact
```

## Why This Project Exists

This project started from a real problem I experienced while travelling: needing quick access to Wi-Fi without mobile data in a new country.

I revisited the project as a way to modernise an older codebase and demonstrate stronger backend engineering practices, including secure configuration, authentication, role-based authorisation, DTO boundaries, encrypted credential storage, automated testing, and CI.

The current version is intentionally a foundation rather than a complete product, with a clear roadmap for the workflows and safeguards that would be needed before treating it as production-ready.
