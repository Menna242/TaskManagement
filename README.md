# TaskManagement API

A task and project management API built with **.NET 8**, demonstrating **Clean Architecture**, **Feature-Based organization**, **CQRS with MediatR**, and **Pipeline Behaviors** for cross-cutting concerns.

This project was built as part of Sprint 4 - moving from a traditional N-Tier architecture to a modern, enterprise-style .NET architecture.

---

## Architecture Overview

The solution is split into **four independent projects**, each with a single, well-defined responsibility:

- **Domain** — core entities, no dependencies
- **Application** — Commands/Queries, DTOs, validation
- **Infrastructure** — EF Core, Repository, UnitOfWork
- **API** — Controllers, Swagger

```
Client → API → MediatR → Application (Handler) → Infrastructure (Repository/DbContext) → Database
```

Controllers never talk to the database or repositories directly. Every request flows through **MediatR**, keeping HTTP concerns (in API) fully separate from business logic (in Application) and technical details (in Infrastructure).

---

## Project Structure

```
TaskManagement/
│
├── TaskManagement.Domain/
│   ├── Common/
│   │   └── BaseEntity.cs
│   └── Entities/
│       ├── TaskItem.cs
│       ├── ProjectEntity.cs
│       └── Comment.cs
│
├── TaskManagement.Application/
│   ├── Abstractions/
│   │   └── Persistence/
│   │       ├── IRepository.cs
│   │       └── IUnitOfWork.cs
│   ├── Common/
│   │   └── Behaviors/
│   │       ├── LoggingBehavior.cs
│   │       └── ValidationBehavior.cs
│   ├── Features/
│   │   ├── Tasks/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTask/
│   │   │   │   ├── UpdateTask/
│   │   │   │   └── DeleteTask/
│   │   │   ├── Queries/
│   │   │   │   ├── GetTasks/
│   │   │   │   └── GetTaskById/
│   │   │   └── Dtos/
│   │   │       └── TaskDto.cs
│   │   └── Projects/
│   │       ├── Commands/
│   │       │   └── CreateProject/
│   │       ├── Queries/
│   │       │   └── GetProjects/
│   │       └── Dtos/
│   │           └── ProjectDto.cs
│   └── DependencyInjection.cs
│
├── TaskManagement.Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/
│   │   │   └── TaskItemConfiguration.cs
│   │   ├── Repositories/
│   │   │   └── Repository.cs
│   │   └── UnitOfWork.cs
│   └── DependencyInjection.cs
│
└── TaskManagement.API/
    ├── Controllers/
    │   ├── TasksController.cs
    │   └── ProjectsController.cs
    ├── appsettings.json
    └── Program.cs
```


Each Feature is self-contained — Commands, Queries, DTOs, and Validators for a use case live together.

---

## Tech Stack

.NET 8 · EF Core 8 (SQL Server) · MediatR 12.4.1 · FluentValidation 11.9.2 · Swagger

---

## Features

- Full CRUD for Tasks (Create, Update, Delete, Get, GetById)
- Create/Get for Projects
- CQRS: Commands write, Queries read only
- Pipeline Behaviors: request logging + execution time, automatic FluentValidation before handlers run
- DTOs only — no EF entities exposed via API

---

## Run

1. Set connection string in `appsettings.json`
2. `Update-Database -StartupProject TaskManagement.API`
3. Run `TaskManagement.API` (F5) → Swagger opens automatically