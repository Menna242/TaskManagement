# TaskManagement API

A task and project management API built with **.NET 8**, demonstrating **Clean Architecture**, **Feature-Based organization**, **CQRS with MediatR**, **Pipeline Behaviors**, **JWT authentication**, **resource-based authorization**, **automated testing**, and **background processing with Hangfire and Quartz.NET**.

- **Sprint 4** established the architectural foundation (Clean Architecture, CQRS, MediatR, Pipeline Behaviors).
- **Sprint 5** extended it with business rules, a task workflow, authentication, authorization, validation, and consistent error handling.
- **Sprint 6** improved reliability with unit and integration tests, and moved non-request work into background and scheduled jobs.

---

## Architecture Overview

The solution is split into **four independent projects** plus a test project, each with a single, well-defined responsibility:

- **Domain**: core entities, enums, domain exceptions, `Result`/`Error` types. No external dependencies.
- **Application**: Commands/Queries, DTOs, validators, pipeline behaviors, background job logic, and abstractions (repositories, auth services, background job service).
- **Infrastructure**: EF Core, ASP.NET Core Identity, JWT token generation, Hangfire and Quartz.NET integration, repository and unit of work implementations.
- **API**: controllers, Swagger, JWT bearer setup, global exception handling, Hangfire dashboard.
- **Tests**: unit tests and integration tests.

```
Client -> API -> MediatR -> Logging -> Validation -> Handler -> Repository abstraction -> Infrastructure -> Database
```

Dependency direction: `API -> Infrastructure -> Application -> Domain`.

Controllers never access repositories or the DbContext directly. Every request flows through **MediatR**, so HTTP concerns (API), business rules (Application), and technical details (Infrastructure) stay separated.

---

## Project Structure

```
TaskManagement/
│
├── TaskManagement.Domain/
│   ├── Common/
│   │   ├── BaseEntity.cs            (generic BaseEntity<TId> + Guid-based BaseEntity)
│   │   ├── Error.cs
│   │   └── Result.cs
│   ├── Entities/
│   │   ├── TaskItem.cs
│   │   ├── ProjectEntity.cs
│   │   └── Comment.cs
│   ├── Enums/
│   │   └── TaskItemStatus.cs
│   └── Exceptions/
│       └── InvalidStatusTransitionException.cs
│
├── TaskManagement.Application/
│   ├── Abstractions/
│   │   ├── Persistence/
│   │   │   ├── IRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   ├── Authentication/
│   │   │   ├── ITokenService.cs
│   │   │   ├── IIdentityService.cs
│   │   │   └── ICurrentUserService.cs
│   │   └── BackgroundJobs/
│   │       └── IBackgroundJobService.cs
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── ValidationBehavior.cs
│   │   └── Exceptions/
│   │       └── ForbiddenAccessException.cs
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/ (Register, Login, RefreshToken, Logout)
│   │   │   └── Dtos/AuthResponse.cs
│   │   ├── Projects/
│   │   │   ├── Commands/ (CreateProject, UpdateProject, DeleteProject)
│   │   │   ├── Queries/  (GetProjects, GetProjectById)
│   │   │   └── Dtos/
│   │   ├── Tasks/
│   │   │   ├── Commands/ (CreateTask, UpdateTask, UpdateTaskStatus, AssignTask, DeleteTask)
│   │   │   ├── Queries/  (GetTasks, GetTaskById)
│   │   │   ├── Jobs/     (TaskCompletedJob)
│   │   │   └── Dtos/
│   │   └── Comments/
│   │       ├── Commands/ (CreateComment, DeleteComment)
│   │       ├── Queries/  (GetTaskComments)
│   │       └── Dtos/
│   └── DependencyInjection.cs
│
├── TaskManagement.Infrastructure/
│   ├── Identity/
│   │   ├── ApplicationUser.cs
│   │   ├── ApplicationRole.cs
│   │   ├── RefreshToken.cs
│   │   ├── IdentityService.cs
│   │   └── IdentitySeeder.cs
│   ├── Authentication/
│   │   ├── JwtSettings.cs
│   │   ├── TokenService.cs
│   │   └── CurrentUserService.cs
│   ├── BackgroundJobs/
│   │   ├── BackgroundJobService.cs   (Hangfire enqueue)
│   │   └── OverdueTasksJob.cs        (Quartz recurring job)
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   └── UnitOfWork.cs
│   ├── AppDbContext.cs
│   └── DependencyInjection.cs
│
├── TaskManagement.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProjectsController.cs
│   │   ├── TasksController.cs
│   │   └── CommentsController.cs
│   ├── Middleware/
│   │   ├── GlobalExceptionHandler.cs
│   │   └── HangfireBasicAuthFilter.cs
│   ├── appsettings.json
│   └── Program.cs
│
└── TaskManagement.Tests/
    ├── Domain/
    │   └── TaskStatusTransitionTests.cs
    ├── Application/
    │   ├── CreateTaskTests.cs
    │   ├── CreateCommentTests.cs
    │   └── AssignTaskTests.cs
    └── Integration/
        ├── ApiFactory.cs
        └── TaskWorkflowTests.cs
```

Each feature is self-contained: its commands, queries, DTOs, validators, and jobs live together.

---

## Tech Stack

.NET 8 · EF Core 8 (SQL Server) · ASP.NET Core Identity · JWT Bearer · MediatR · FluentValidation · Hangfire · Quartz.NET · xUnit · NSubstitute · WebApplicationFactory · Swagger

---

## Features

### Core (Sprint 4)
- CQRS: commands write, queries only read
- Pipeline Behaviors: request logging with execution time, and automatic FluentValidation before handlers run
- DTOs only: no EF entities exposed through the API

### Business Rules
- Full CRUD for Projects, Tasks, and Comments (Comments: add, list by task, delete)
- Every Task belongs to an existing Project, and every Comment belongs to an existing Task
- Tasks can be assigned to an existing user; assigning to a user that does not exist is rejected
- Tasks can have an optional due date
- Required fields: Project name, Task title, Comment content
- Deleting a Project cascades to its Tasks, and deleting a Task cascades to its Comments

### Task Workflow
Tasks follow a controlled lifecycle: `Todo`, `InProgress`, `Completed`, `Cancelled`. The transition rules live inside the `TaskItem` entity.

| From | Allowed transitions |
|---|---|
| Todo | InProgress, Cancelled |
| InProgress | Completed, Cancelled |
| Completed | none |
| Cancelled | none |

Invalid transitions are rejected with a `409 Conflict`. Status changes use a dedicated use case (`UpdateTaskStatus`).

### Authentication
- ASP.NET Core Identity with `ApplicationUser`
- Register, Login, Refresh Token (with rotation), and Logout
- JWT access tokens with claims: `sub`, `email`, `role`
- Refresh tokens are stored as SHA-256 hashes and revoked on logout

### Authorization
- Roles: `Admin` and `User` (seeded at startup)
- New users receive the `User` role
- `[Authorize]` on all secured controllers
- Resource-based ownership checks in handlers: users can only modify their own Projects, Tasks, and Comments. Admins can access any resource.

### Validation and Error Handling
- FluentValidation validators for CreateProject, UpdateProject, CreateTask, UpdateTask, and CreateComment, executed through the existing `ValidationBehavior`
- `Result<T>` used for expected failures (for example, `GetTaskById`)
- `ProblemDetails` for all API errors
- Global exception handling via `IExceptionHandler`

| Status | Meaning |
|---|---|
| 400 | Validation failed |
| 401 | Missing or invalid token |
| 403 | Not allowed to access the resource |
| 404 | Resource not found |
| 409 | Conflict (for example, invalid status transition) |
| 500 | Unexpected server error |

---

## Testing (Sprint 6)

Tests focus on observable behavior and use the Arrange / Act / Assert structure. Test names describe behavior, for example `Should_Complete_Task_When_CurrentStatus_IsInProgress`.

### Unit tests
Mocks (NSubstitute) are used only for external collaborators such as the unit of work and the identity service. The five business-rule areas covered:

| Area | Test class |
|---|---|
| Valid Task status transition | `TaskStatusTransitionTests` |
| Invalid Task status transition | `TaskStatusTransitionTests` |
| Creating a Task with an invalid Project | `CreateTaskTests` |
| Adding a Comment to an invalid Task | `CreateCommentTests` |
| Assigning a Task to an invalid User | `AssignTaskTests` |

Both success and failure paths are covered. Status transition tests need no mocks because the rules live in the `TaskItem` entity.

### Integration tests
Integration tests run the whole API in memory through `WebApplicationFactory` and call it with `HttpClient`.

The critical workflow tested end to end:

```
Register -> Login -> Create Project -> Create Task -> Assign Task -> Complete Task
```

- Ids and the access token are passed from one step to the next, nothing is hard-coded.
- Status codes and key response data are asserted at each step.
- A dedicated SQL Server database (`TaskManagementDb_Tests`) is deleted and recreated for each test run, so tests are isolated and repeatable.
- `IBackgroundJobService` is replaced with a test double, so the workflow does not depend on a running Hangfire worker.

Run all tests from Visual Studio (**Test > Run All Tests**) or with:

```
dotnet test
```

The test database connection string is defined in `TaskManagement.Tests/Integration/ApiFactory.cs`. Adjust it if your SQL Server instance differs.

---

## Background Processing (Sprint 6)

### Hangfire: event-driven jobs
When a Task is completed, `UpdateTaskStatus` enqueues a background job instead of doing the work inside the HTTP request.

```
Task Completed -> Enqueue Job (stored in SQL Server) -> Hangfire Worker -> TaskCompletedJob -> Structured Log
```

- **Persistent storage:** jobs are stored in SQL Server, so they survive an application restart.
- **Lightweight payload:** only the `taskId` is passed. The job loads the current state from the database when it runs.
- **Scoped services:** the job receives its dependencies through the DI scope created for its execution.
- **Retries:** 3 automatic attempts with delays of 10, 30, and 60 seconds. After that the job stays in the **Failed** state for inspection and can be requeued from the dashboard.
- **Idempotency:** the job checks `CompletionNotifiedAt` before notifying, so running it twice does not send the notification twice.
- **Dashboard:** available at `/hangfire`, protected with Basic Authentication. Credentials come from configuration:

```json
"Hangfire": {
  "Username": "admin",
  "Password": "change-me"
}
```

The notification itself is simulated with structured logging.

### Quartz.NET: scheduled jobs
`OverdueTasksJob` runs on a recurring schedule, finds overdue Tasks (due date in the past and status not `Completed` or `Cancelled`), and logs the number of processed records and the details of each one.

```
Scheduled Trigger -> OverdueTasksJob -> Find Overdue Tasks -> Structured Log
```

- Configured through ASP.NET Core dependency injection (`AddQuartz` and `AddQuartzHostedService`).
- The `CancellationToken` is passed to the database query and cancellation is handled explicitly.
- Expected failures are caught and logged without stopping the schedule.
- `[DisallowConcurrentExecution]` prevents overlapping runs.
- The schedule interval is defined in `Infrastructure/DependencyInjection.cs`.

### When to use which
- **Hangfire:** work triggered by a business event, with persistence, retries, and a dashboard.
- **Quartz.NET:** work that must run on a time schedule, regardless of user actions.

---

## API Endpoints

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `/login`, `/refresh`, `/logout` |
| Projects | `GET /api/projects`, `GET /api/projects/{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` |
| Tasks | `GET /api/tasks`, `GET /api/tasks/{id}`, `POST`, `PUT /{id}`, `PATCH /{id}/status`, `PUT /{id}/assign`, `DELETE /{id}` |
| Comments | `POST /api/tasks/{taskId}/comments`, `GET /api/tasks/{taskId}/comments`, `DELETE /api/comments/{id}` |
| Dashboard | `GET /hangfire` (Basic Auth) |

All endpoints except `/api/auth/*` require a valid access token.

---

## Run

1. Set the connection string, the `Jwt` section, and the `Hangfire` section in `TaskManagement.API/appsettings.json`. `Jwt:Key` must be at least 32 characters.
2. Apply migrations:
   ```
   Update-Database
   ```
   (Package Manager Console, with `TaskManagement.Infrastructure` as the default project and `TaskManagement.API` as the startup project.) Hangfire creates its own tables on first start.
3. Run `TaskManagement.API` (F5). Swagger opens automatically.
4. Call `POST /api/auth/register`, copy the `accessToken`, click **Authorize** in Swagger, and paste the token (without the `Bearer` prefix).
5. Complete a Task (`InProgress`, then `Completed`) and watch the job in the Hangfire dashboard and in the application logs.
6. Create a Task with a `dueDate` in the past and watch the Quartz job report it in the logs.

To create an Admin, register a user and then assign the `Admin` role to it in the `AspNetUserRoles` table.

---

## Request Flow

```
HTTP request -> Controller -> MediatR -> LoggingBehavior -> ValidationBehavior -> Handler
   -> Repository abstraction -> Infrastructure -> DbContext -> SQL Server

Task completed -> Handler -> Hangfire enqueue -> Worker -> TaskCompletedJob
Schedule -> Quartz -> OverdueTasksJob -> Repository -> Structured log
```
