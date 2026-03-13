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

# Run all unit tests
dotnet test

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

The solution has two projects:
- `FbRider.Api` — main API
- `FbRider.Api.Tests.Unit` — NUnit unit tests (using Moq for mocking)

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

- **Controllers** — thin; delegate to services
- **Services** (`/Services`) — orchestration and business logic; call Yahoo API clients and repositories
- **Domain** (`/Domain`) — pure domain models and `AllPlayScoreService` (all-play scoring logic, no external dependencies)
- **YahooApi** (`/YahooApi`) — HTTP clients for Yahoo OAuth and Fantasy Sports endpoints; `FantasySportsApiResources.cs` is the large resource model file mapping Yahoo API responses
- **Repositories** (`/Repositories`) — MongoDB data access for `UserToken` (OAuth tokens per user)
- **Middlewares** — `TokenRefreshMiddleware` automatically refreshes expired Yahoo OAuth tokens; `GlobalExceptionHandler` handles unhandled exceptions

### Authentication

Yahoo OAuth2/OIDC flow: frontend redirects to Yahoo → Yahoo calls back to `POST /api/yahooauth/callback` → tokens stored in MongoDB `UserTokens` collection → cookie session (30-day, HttpOnly, SameSite=None for cross-origin).

### Configuration

- **MongoDB**: `mongodb://admin:password@localhost:27017`, database `FbRiderDb`
- **Frontend**: `https://localhost:3000` (CORS allowed origin)
- **Secrets**: Managed via .NET User Secrets (ID: `35a9f342-ebd2-40ec-90c1-984312ca244a`) in development
- **Logging**: Serilog to console and rolling daily files at `/logs/log-*.txt`

### Test Patterns

Tests use builder classes in `FbRider.Api.Tests.Unit/Data/Builders/` to construct test data. When adding tests, follow the builder pattern for domain model construction.
