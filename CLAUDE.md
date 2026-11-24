# TRS backend

.NET 10 solution: `TRS-API` (ASP.NET Core Web API, controllers) references `TRS-Lib` (models, DTOs, DbContext, repositories, services).

## Conventions

- Models go in `TRS-Lib/Models`, request/response DTOs in `TRS-Lib/Dtos`. Never return anonymous objects from controllers; add a DTO so Swagger can describe it.
- Every endpoint requires the `X-API-KEY` header unless the controller or action has `[AllowWithoutApiKey]`.
- Endpoints that need a logged in user get `[Authorize]` and read the user id from the `sub` claim (`User.FindFirstValue(JwtRegisteredClaimNames.Sub)`). Access tokens last 60 minutes, refresh tokens 30 days and are rotated on every use (`TRS-Lib/Services/TokenService.cs`).
- Every controller action must be documented for Swagger:
  - `/// <summary>` and, where useful, `/// <remarks>` on the action
  - `/// <response code="...">` plus a matching `[ProducesResponseType]` for each status code it can return (typed with `<T>` when there is a body)
  - `[Consumes("application/json")]` on actions with a body; `[Produces("application/json")]` and `[Tags("...")]` on the controller
  - `/// <summary>` and `/// <example>` on DTO properties
  - The 401 response and the API key / bearer token requirements are added automatically by `TRS-API/OpenApi/SecurityTransformer.cs`; don't declare them by hand.
- Swagger UI: `/swagger`. OpenAPI document: `/openapi/v1.json`.

## Database

- Postgres on Railway: only ever touch the `trs-db` service in project `trailrunningsweden.se`. Never any other database.
- On Railway the app connects using the `DATABASE_URL` variable (converted in `TRS-Lib/Data/DatabaseUrl.cs`). `ConnectionStrings:Default` takes precedence when set (locally via appsettings.Development.json). Don't ask for new Railway variables when an existing one can be used.
- Migrations live in `TRS-Lib/Migrations`:
  `dotnet ef migrations add <Name> --project TRS-Lib --startup-project TRS-API --output-dir Migrations`
- Apply through an SSH tunnel (the database has no public address):
  `railway connect trs-db --tunnel-only --port 15432 --project 259ef35e-d6c9-4b5c-81e6-10ba15c35813 --environment production`
