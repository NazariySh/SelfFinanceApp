# SelfFinance WebApi

ASP.NET Core 8 Web API for tracking personal income and expenses.

## What it does

- **Accounts** — registration, login, JWT access/refresh tokens, role management (User/Admin), admin endpoints for managing all accounts.
- **Financial operations** — create, edit, delete, and list income/expense entries scoped to the logged-in user, with pagination and date/date-range filtering.
- **Financial reports** — daily and period summaries built from a user's financial operations.

## Tech stack

- ASP.NET Core 8 Web API, EF Core 8 (SQL Server) + ASP.NET Core Identity for auth
- JWT bearer authentication with access/refresh tokens
- AutoMapper for entity/DTO mapping, FluentValidation for request validation
- xUnit test suite (EF Core in-memory provider) covering the Application service layer

## Project layout

- `src/Domain` — entities, repository interfaces, domain exceptions
- `src/Application` — services, DTOs mapping profiles, validators (business logic)
- `src/Infrastructure` — EF Core `DbContext`, repositories, Identity wiring
- `src/WebApi` — controllers, JWT/auth setup, middleware, composition root
- `tests/SelfFinanceWebApi.Tests` — unit tests for the Application service layer

## Running locally

```bash
dotnet restore
dotnet run --project src/WebApi
```

Configure the SQL Server connection string and `JwtSettings` in `src/WebApi/appsettings.Development.json` before running.

## Running tests

```bash
dotnet test tests/SelfFinanceWebApi.Tests/SelfFinanceWebApi.Tests.csproj
```
