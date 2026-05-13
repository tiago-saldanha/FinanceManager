# Finanza

![Build](https://github.com/tiago-saldanha/Finanza/actions/workflows/ci.yml/badge.svg)
![Coverage](https://codecov.io/gh/tiago-saldanha/Finanza/branch/master/graph/badge.svg)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Finanza is a full-stack personal finance management application built with ASP.NET Core (.NET 8) and Angular 18, following Clean Architecture and Domain-Driven Design (DDD) principles.

---

## 🏗 Architecture

### Backend

The backend is structured using layered Clean Architecture:

#### 🔹 Domain
- Entities, Value Objects, Domain Services, Domain Events
- Business rules with no external dependencies

#### 🔹 Application
- Application Services and DTOs
- Interfaces (Repositories, Unit of Work)
- Orchestration logic

#### 🔹 Infrastructure
- Entity Framework Core with SQLite
- Repository and Unit of Work implementations
- Identity (ASP.NET Core Identity + JWT)
- Email service via Resend API
- Multi-tenant database provisioning

#### 🔹 API
- Minimal APIs
- JWT authentication
- Swagger / OpenAPI
- Problem Details middleware

### Frontend (`Finanza.Client`)

Angular 18 SPA with standalone components:

- **Dashboard** — charts and financial summary by category
- **Transactions** — list, create, edit, pay, reopen and cancel transactions
- **Categories** — create and manage spending/revenue categories
- **Auth** — login, register, forgot/reset password, change password
- **Account** — profile management, data export and account deletion
- Light/dark theme support

---

## 🚀 Technologies

### Backend
- .NET 8 / ASP.NET Core
- Entity Framework Core + SQLite
- ASP.NET Core Identity + JWT
- Resend (transactional email)

### Frontend
- Angular 18 (standalone components)
- Angular Material
- Chart.js + ng2-charts
- Reactive Forms

### Tests
- xUnit + FluentAssertions
- Moq
- WebApplicationFactory (integration tests)

---

## 🧠 Architectural Patterns

- Clean Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- Unit of Work
- Domain Events
- Multi-tenancy (per-user SQLite databases)
- Dependency Injection

---

## 🧪 Testing Strategy

### ✔ Domain Tests
Unit tests for entities, value objects, domain services and business rules.

### ✔ Application Tests
Unit tests for application services and use case orchestration using Moq.

### ✔ Infrastructure Tests
Integration tests for repositories and auth services against a real SQLite database.

### ✔ API Integration Tests
End-to-end tests using `WebApplicationFactory` with in-memory SQLite and the full DI container.

---

## 💾 Database

Each user gets an isolated SQLite database (multi-tenant by user). A shared `app.db` manages tenant routing.

For integration tests, SQLite in-memory is used and the schema is created automatically during test setup.

---

## ▶ Running the Application

### API

```bash
cd src/Finanza.API
dotnet run
```

### Frontend

```bash
cd src/Finanza.Client
npm install
npm start
```

The frontend runs on `http://localhost:4200` and the API on `https://localhost:7XXX` (see `launchSettings.json`).

---

## 🧪 Running Tests

```bash
dotnet test
```

---

## 👨‍💻 Author

Tiago Ávila Saldanha — .NET Full Stack Developer

---

## 📄 License

This project is for educational and portfolio purposes.
