# User Management API

An ASP.NET Core 9 Web API for the TechHive Solutions internal HR and IT user-management exercise. It provides CRUD endpoints for user records and validates request data.

## Requirements

- .NET 9 SDK

## Run locally

From this directory, configure a local development token and start the HTTP launch profile:

```powershell
dotnet restore
$env:Authentication__BearerToken = "replace-with-a-long-random-token"
dotnet run --launch-profile http
```

The `http` profile listens on localhost for local development. Use HTTPS for any non-local deployment, and provide the token through environment configuration or a managed secret store. The API uses an in-memory store, so records are cleared when the application stops.

For Bash, set the token for the `dotnet run` process like this:

```sh
Authentication__BearerToken="replace-with-a-long-random-token" dotnet run --launch-profile http
```

The API fails closed with `503 Service Unavailable` if no token is configured, and returns `401 Unauthorized` when a request has a missing or invalid token. Send the configured value in the `Authorization: Bearer <token>` header. The shared-token middleware is a training example; production authentication should use a standards-based identity provider and JWT bearer validation, with secrets stored in a managed secret store.

## Endpoints

| Method | Route | Behavior |
| --- | --- | --- |
| `GET` | `/api/users` | Return all users |
| `GET` | `/api/users/{id}` | Return one user or `404` |
| `POST` | `/api/users` | Create a user; returns `201` and a location header |
| `PUT` | `/api/users/{id}` | Replace a user’s details or return `404` |
| `DELETE` | `/api/users/{id}` | Delete a user; returns `204` or `404` |

Example request body for `POST` and `PUT`:

```json
{
  "name": "Jordan Lee",
  "email": "jordan.lee@example.com",
  "department": "IT"
}
```

Requests are validated for required values, non-whitespace names and departments, string lengths, and email format. Invalid request bodies receive `400 Bad Request` through `[ApiController]` model validation. Middleware logs method, path, status, and duration without logging request bodies or tokens. Exception handling returns a generic JSON `500` response for unhandled server errors.

## Project structure

```text
Controllers/UsersController.cs  HTTP routes and status codes
Models/User.cs                  User response model
Models/UserWriteRequest.cs      Validated create/update input
Services/UserStore.cs           In-memory CRUD operations
Middleware/RequestLoggingMiddleware.cs       Request/response metadata logging
Middleware/ExceptionHandlingMiddleware.cs   Consistent JSON exception response
Middleware/ApiTokenAuthenticationMiddleware.cs Bearer token validation
Program.cs                      Dependency registration and API startup
```

## Testing with a REST client

Set the token environment variable, start the API, and use Postman, VS Code REST Client, or `curl` to exercise each route. The `UserManagementAPI.http` file contains requests for listing, creating, retrieving, updating, deleting, invalid input, and an invalid token. Replace its token placeholder with the configured value. Create a user first and use the returned `id` in the subsequent requests.

## Debugging notes

See [DEBUGGING.md](DEBUGGING.md) for the review findings, fixes, middleware order, and edge-case checks.

## AI assistance record

This project was prepared with OpenAI Codex in this workspace; 
