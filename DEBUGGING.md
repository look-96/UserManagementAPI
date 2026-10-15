# Debugging and reliability notes

## Findings and changes

- **Weak input validation:** Create and update now share validation rules for required values, email format, length, and whitespace-only name or department values. `[ApiController]` returns `400 Bad Request` when validation fails.
- **Missing users:** Item lookup, update, and delete return `404 Not Found` for unknown IDs instead of failing during a lookup.
- **Unhandled exceptions:** `ExceptionHandlingMiddleware` is configured early in the pipeline so unexpected server errors receive a generic JSON response rather than implementation details.
- **List performance:** `UserStore` uses a `ConcurrentDictionary<int, User>` for average constant-time ID lookups and avoids sorting the full collection on every list request. Listing remains linear in the number of users because every record must be returned.
- **Storage limitation:** The store is in memory and does not persist records across restarts. A database and paging should replace or extend it for production-scale data.
- **Request auditing:** Request logging records method, path, final status, and elapsed time, including authentication failures. It deliberately excludes request bodies and authorization headers to avoid logging sensitive data.
- **Consistent errors:** Exception handling returns JSON with a generic `500` message and logs the exception details server-side.
- **Token enforcement:** Every route requires the configured bearer token; missing or invalid tokens receive `401`. Missing server-side token configuration fails closed with `503`.

## Middleware order

The pipeline is exception handling, request logging, HTTPS redirection, bearer-token authentication, then controller endpoints. Logging runs before authentication so denied requests are audited. The logger registers an `OnCompleted` callback to capture the final status after the exception handler has produced its response. This ordering nuance matters: putting authentication before logging would short-circuit invalid-token requests before the logger sees them.

## Edge-case request checks

`UserManagementAPI.http` includes normal CRUD requests, invalid input, and an invalid token. Set `Authentication__BearerToken`, replace the request file's token placeholder, and run the requests in order using VS Code REST Client or adapt them in Postman. Expected statuses include `201` for create, `200` for reads and updates, `204` for delete, `400` for invalid input, `401` for an invalid token, and `404` for unknown IDs. To verify exception handling, temporarily add a controlled test endpoint that throws, confirm the generic JSON `500`, then remove that endpoint.

The project was restored and built successfully in this workspace. Live localhost HTTP checks could not be completed here because the execution environment blocked or timed out on connections to the local server; rerun the included requests in your local development environment.

## AI assistance record

This review and the listed changes were produced with OpenAI Codex in this workspace. 
