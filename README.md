# TaskManagement API

A task and project management API built with **.NET 8**, demonstrating **Clean Architecture**, **Feature-Based organization**, **CQRS with MediatR**, **Pipeline Behaviors**, **JWT authentication**, and **resource-based authorization**.

Sprint 4 established the architectural foundation (Clean Architecture, CQRS, MediatR, Pipeline Behaviors). Sprint 5 extends that same system with business rules, a task workflow, authentication, authorization, validation, and consistent error handling.

---

## Architecture Overview

The solution is split into **four independent projects**, each with a single, well-defined responsibility:

- **Domain**: core entities, enums, `Result`/`Error` types. No external dependencies.
- **Application**: Commands/Queries, DTOs, validators, pipeline behaviors, and abstractions (repositories, auth services).
- **Infrastructure**: EF Core, ASP.NET Core Identity, JWT token generation, repository and unit of work implementations.
- **API**: controllers, Swagger, JWT bearer setup, global exception handling.

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
│   └── Entities/
│       ├── TaskItem.cs
│       ├── ProjectEntity.cs
│       ├── Comment.cs
│       └── TaskStatus.cs
│
├── TaskManagement.Application/
│   ├── Abstractions/
│   │   ├── Persistence/
│   │   │   ├── IRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── Authentication/
│   │       ├── ITokenService.cs
│   │       ├── IIdentityService.cs
│   │       └── ICurrentUserService.cs
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
│   │   │   ├── Commands/ (CreateTask, UpdateTask, UpdateTaskStatus, DeleteTask)
│   │   │   ├── Queries/  (GetTasks, GetTaskById)
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
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   └── UnitOfWork.cs
│   ├── AppDbContext.cs
│   └── DependencyInjection.cs
│
└── TaskManagement.API/
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── ProjectsController.cs
    │   ├── TasksController.cs
    │   └── CommentsController.cs
    ├── Middleware/
    │   └── GlobalExceptionHandler.cs
    ├── appsettings.json
    └── Program.cs
```

Each feature is self-contained: its commands, queries, DTOs, and validators live together.

---

## Tech Stack

.NET 8 · EF Core 8 (SQL Server) · ASP.NET Core Identity · JWT Bearer · MediatR · FluentValidation · Swagger

---

## Features

### Core (Sprint 4)
- CQRS: commands write, queries only read
- Pipeline Behaviors: request logging with execution time, and automatic FluentValidation before handlers run
- DTOs only: no EF entities exposed through the API

### Business Rules
- Full CRUD for Projects, Tasks, and Comments (Comments: add, list by task, delete)
- Every Task belongs to an existing Project, and every Comment belongs to an existing Task
- Required fields: Project name, Task title, Comment content
- Deleting a Project cascades to its Tasks, and deleting a Task cascades to its Comments

### Task Workflow
Tasks follow a controlled lifecycle: `Todo`, `InProgress`, `Completed`, `Cancelled`.

| From | Allowed transitions |
|---|---|
| Todo | InProgress, Cancelled |
| InProgress | Completed, Cancelled |
| Completed | none |
| Cancelled | none |

Invalid transitions are rejected with an error response. Status changes use a dedicated use case (`UpdateTaskStatus`).

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

## API Endpoints

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `/login`, `/refresh`, `/logout` |
| Projects | `GET /api/projects`, `GET /api/projects/{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` |
| Tasks | `GET /api/tasks`, `GET /api/tasks/{id}`, `POST`, `PUT /{id}`, `PATCH /{id}/status`, `DELETE /{id}` |
| Comments | `POST /api/tasks/{taskId}/comments`, `GET /api/tasks/{taskId}/comments`, `DELETE /api/comments/{id}` |

All endpoints except `/api/auth/*` require a valid access token.

---

## Run

1. Set the connection string and the `Jwt` section in `TaskManagement.API/appsettings.json`. `Jwt:Key` must be at least 32 characters.
2. Apply migrations:
   ```
   Update-Database
   ```
   (Package Manager Console, with `TaskManagement.Infrastructure` as the default project and `TaskManagement.API` as the startup project.)
3. Run `TaskManagement.API` (F5). Swagger opens automatically.
4. Call `POST /api/auth/register`, copy the `accessToken`, click **Authorize** in Swagger, and paste the token (without the `Bearer` prefix).

To create an Admin, register a user and then assign the `Admin` role to it in the `AspNetUserRoles` table.

---

## Request Flow

```
HTTP request -> Controller -> MediatR -> LoggingBehavior -> ValidationBehavior -> Handler
   -> Repository abstraction -> Infrastructure -> DbContext -> SQL Server
```
