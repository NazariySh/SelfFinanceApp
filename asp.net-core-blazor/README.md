# SelfFinance Blazor App

Blazor Server front end for the [SelfFinance WebApi](../asp-backend-webapi) — a personal income/expense tracker.

## What it does

- **Auth** — login/registration, JWT-based session backed by the WebApi, automatic token refresh, logout.
- **Profile** — view and edit the logged-in user's account details.
- **Operations** — create, edit, delete, and browse paginated financial (income/expense) entries.
- **Reports** — view daily and date-range financial reports.
- **Admin** — manage all user accounts, roles, and financial operations across users.

## Tech stack

- ASP.NET Core 8, Blazor Server (interactive server render mode)
- Typed `HttpClient` services calling the SelfFinance WebApi, wrapped with an auth handler that attaches/refreshes JWTs automatically
- `ProtectedSessionStorage` for persisting tokens client-side

## Project layout

- `src/Domain`, `src/Shared` — models/DTOs shared with the API contract
- `src/Application` — HTTP client services, auth state provider, token refresh/error handling
- `src/BlazorSelfFinanceApp` — Razor components, pages, and the app host

## Running locally

```bash
dotnet restore
dotnet run --project src/BlazorSelfFinanceApp
```

Requires the SelfFinance WebApi running and reachable at the base URL configured under `WebApi:SelfFinanceApi:BaseAddress` in `src/BlazorSelfFinanceApp/appsettings.json`.
