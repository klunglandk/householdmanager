# HouseholdManager

A self-hosted household app for a small family: shared chores with recurrence,
a shopping list, recipes and a weekly school plan. The backend is an
ASP.NET Core Web API. A React/PWA frontend is planned.

> Status: backend feature-complete for a first version, running on an
> in-memory database. PostgreSQL support and a frontend are next.

## Features

- **Chores** with daily, weekly, monthly and yearly recurrence. Instances are
  generated ahead of time by a background service. Individual occurrences can
  be skipped, and a recurrence can be ended or changed from tomorrow.
- **Shopping list** that merges duplicate items and converts between units
  (for example 2 dl + 3 dl becomes 0.5 l). Items can be added from a recipe,
  scaled to a number of servings.
- **Recipes** with ingredients and an optional image.
- **School plan** uploads (PDF or image) per week.
- **Accounts and roles**: an admin (adult) and a child with limited rights.

## Tech stack

- .NET 10, ASP.NET Core Web API
- Entity Framework Core (in-memory for now, PostgreSQL planned)
- ASP.NET Core Identity with roles
- JWT access tokens and rotating refresh tokens
- Docker

## Architecture

```
Controllers   HTTP endpoints, authorization attributes
Services      business logic, one service per area
Database      EF Core context and data models
Dtos / Models responses and request models
Infrastructure  options, file storage, current user, background service
```

The signed-in user and their household come from the access token
(`ICurrentUser`), never from the request. Every query is filtered on the
household, so one household cannot read or change another one's data.

## Roles

| Action                                         | Admin | Child |
|------------------------------------------------|:-----:|:-----:|
| Create users, reset passwords                  |  yes  |       |
| Create, edit and end chores                    |  yes  |       |
| Tick off chores                                |  yes  | own only |
| View recipes and the school plan               |  yes  |  yes  |
| Add items to the shopping list                 |  yes  |  yes  |
| Edit or delete shopping items, manage recipes  |  yes  |       |

## Security

- Passwords hashed by Identity, account lockout after repeated failures
- Rate limiting on login and refresh
- Refresh tokens are stored hashed, rotated on every use, and revoked on
  password change or when a used token appears again
- Uploaded files are checked by content (magic bytes), not file name, and
  stored under random names
- Secrets live in user secrets or environment variables, never in the repo

## Getting started

Prerequisites: the .NET 10 SDK.

1. Set the secrets (from the project folder):

   ```
   dotnet user-secrets set "Jwt:Key" "<random string, at least 32 characters>"
   dotnet user-secrets set "SeedAdmin:UserName" "<admin user name>"
   dotnet user-secrets set "SeedAdmin:Password" "<admin password>"
   ```

2. Copy `.env.example` to `.env` and fill in the same admin credentials. The
   `.env` file is only used by the `.http` test file and is git-ignored.
3. Run the app:

   ```
   dotnet run
   ```

4. Open `HouseholdManager.http` in VS Code (REST Client) and run the requests
   from the top. It logs in, creates a child account and exercises most
   endpoints.

## Roadmap

- [ ] PostgreSQL with migrations
- [ ] Unit tests (recurrence and unit conversion)
- [ ] React/PWA frontend
- [ ] Docker setup behind a reverse proxy with TLS

## License
MIT, see the LICENSE file.
