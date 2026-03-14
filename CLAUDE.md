# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FbRider is a Fantasy Basketball API service built with ASP.NET Core (.NET 10) that integrates with the Yahoo Fantasy Sports API. It handles OAuth2 authentication, league data retrieval, and all-play standings calculations.

## Commands

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API (HTTP: localhost:5095, HTTPS: localhost:7149)
dotnet run --project FbRider.Api/FbRider.Api.csproj

# Run all unit tests (both test projects)
dotnet test

# Run tests in a specific project
dotnet test FbRider.Api.Tests.Unit/FbRider.Api.Tests.Unit.csproj
dotnet test FbRider.AllPlay.Tests/FbRider.AllPlay.Tests.csproj

# Run a single test class
dotnet test --filter "ClassName=AllPlayScoreServiceTests"

# Run with coverage
dotnet test /p:CollectCoverage=true

# Start MongoDB via Docker Compose
docker-compose up -d

# Build Docker image
docker build -t fbrider-api:latest -f FbRider.Api/Dockerfile .
```

Swagger UI is available at `/swagger` when running in development mode.

## Architecture

The solution has 8 projects:

| Project | Purpose |
|---|---|
| `FbRider.Api` | Main ASP.NET Core web API (controllers, middlewares, DI wiring) |
| `FbRider.Application` | Service interfaces and `UserService` (token/profile management) |
| `FbRider.Domain` | Pure domain models (`League`, `Team`, `Player`, `Season`, etc.) |
| `FbRider.AllPlay` | All-play standings engine: `AllPlayService` + `AllPlayScoreService` |
| `FbRider.YahooApi` | HTTP clients for Yahoo OAuth and Fantasy Sports; `FantasySportsApiResources.cs` contains XML deserialization DTOs for Yahoo API responses |
| `FbRider.Infastructure` | MongoDB `UserTokenRepository` (stores OAuth tokens per user) |
| `FbRider.Api.Tests.Unit` | NUnit + Moq tests for API layer |
| `FbRider.AllPlay.Tests` | NUnit + Moq tests for all-play scoring logic |

### Request Flow

```
HTTP Request
  → Middleware (TokenRefreshMiddleware, GlobalExceptionHandler)
  → Controller (YahooAuthController, UserController, LeaguesController)
  → Service (UserService, LeagueService, AllPlayService)
  → Repository (UserTokenRepository → MongoDB)
      or
  → YahooApi client (IYahooFantasySportsApiClient → Yahoo API)
```

### Key Layers

- **Controllers** — thin; delegate to services. Endpoints: `YahooAuthController` (`/api/yahooauth/*`), `UserController` (`/api/user/*`), `LeaguesController` (`/api/leagues/*`)
- **Application** — `UserService` orchestrates token lifecycle; service interfaces (`IUserService`, `ILeagueService`) live here
- **LeagueService** — implemented in `FbRider.YahooApi`; calls Yahoo API, maps results via AutoMapper, filters out non-NBA games and closed seasons
- **AllPlay** — `AllPlayService` calls Yahoo API per week, creates `PositiveStat`/`NegativeStat` based on stat category `SortOrder` (1 = higher is better, else lower is better), then delegates to `AllPlayScoreService` (pure, no external deps) which computes matchup results and returns `AllPlayStandingsDTO`
- **YahooApi** — `YahooFantasySportsApiClient` and `YahooSignInApiClient`; `FantasySportsApiResources.cs` is ~950 lines of XML DTOs using Yahoo's namespace attributes
- **Mapping** — `YahooApiResourceMappingProfile` in `FbRider.Api/Mapping/` maps `FantasySportsApiResources` to domain models via AutoMapper; all mappings use `.ForAllMembers(opt => opt.Condition(...))` to ignore nulls; string-to-bool conversions check for `"1"`
- **Infastructure** — MongoDB access; note the intentional typo in the project name
- **Middlewares** — `TokenRefreshMiddleware` refreshes expired Yahoo OAuth tokens before the request hits a controller; `GlobalExceptionHandler` catches `YahooApiException` (parses XML/JSON error body) and generic errors, signs out user on 401/403, returns JSON `ApiErrorResponse`

### Authentication

Yahoo OAuth2/OIDC flow: frontend redirects to Yahoo → Yahoo calls back to `POST /api/yahooauth/callback` → tokens stored in MongoDB `UserTokens` collection → cookie session (30-day, HttpOnly, SameSite=None for cross-origin).

### DI Lifetimes

- **Singletons**: `ISignInApiClient`, `IYahooFantasySportsApiClient`
- **Scoped**: `IUserService`, `ILeagueService`, `IUserTokenRepository`

### Configuration

- **MongoDB**: `mongodb://admin:password@localhost:27017`, database `FbRiderDb`
- **Frontend**: `https://localhost:3000` (CORS allowed origin)
- **Secrets**: Managed via .NET User Secrets (ID: `35a9f342-ebd2-40ec-90c1-984312ca244a`) in development
- **Logging**: Serilog to console and rolling daily files at `/logs/log-*.txt`

### Test Patterns

- `FbRider.Api.Tests.Unit` covers controllers, services, middlewares, and AutoMapper mappings.
- `FbRider.AllPlay.Tests` covers all-play domain models and `AllPlayScoreService`.
- Both projects use builder classes (e.g. `LeagueBuilder`, `TeamBuilder`, `PlayerBuilder`) with fluent `With*()` methods to construct test data. Builders populate Yahoo API resource objects (not domain models directly), and static factory properties like `PlayerBuilder.Guard` provide pre-configured instances. When adding tests, follow this builder pattern rather than constructing objects directly.