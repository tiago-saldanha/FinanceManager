# Finanza

![Build](https://github.com/tiago-saldanha/FinanceManager/actions/workflows/ci.yml/badge.svg)
![Coverage](https://codecov.io/gh/tiago-saldanha/FinanceManager/branch/master/graph/badge.svg)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Finanza is a backend application built with ASP.NET Core (.NET 8) following Clean Architecture and Domain-Driven Design (DDD) principles.

The project focuses on maintainability, separation of concerns, and testability, simulating a real-world enterprise-ready architecture.

---

## 🏗 Architecture

The solution is structured using layered architecture:


### 🔹 Domain
- Entities
- Value Objects
- Domain Services
- Domain Events
- Business rules
- No external dependencies

### 🔹 Application
- Application Services
- DTOs
- Interfaces (Repositories, Unit of Work)
- Orchestration logic

### 🔹 Infrastructure
- Entity Framework Core
- SQLite provider
- Repository implementations
- Unit of Work implementation

### 🔹 API
- Minimal APIs
- Dependency Injection setup
- Swagger
- Problem Details middleware

---

## 🚀 Technologies

- .NET 8
- ASP.NET Core
- Entity Framework Core
- SQLite
- xUnit
- FluentAssertions
- WebApplicationFactory (Integration Tests)

---

## 🧠 Architectural Patterns

This project applies:

- Clean Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- Unit of Work
- Domain Events
- Dependency Injection
- Separation of Concerns

---

## 🧪 Testing Strategy

The project includes:

### ✔ Domain Tests
Unit tests validating:
- Entities behavior
- Domain Services
- Business rules

### ✔ Application Tests
Testing:
- Application Services
- Use case orchestration

### ✔ API Integration Tests
Using `WebApplicationFactory` with:
- In-memory SQLite database
- Full request/response pipeline testing
- Real dependency injection container

---

## 💾 Database

The application uses SQLite for development and testing.

For integration tests:
- SQLite in-memory provider is used
- Database schema is created automatically during test setup

---

### 🔹 API Running the Application

```bash
dotnet run
```

---

### 🔹 Running Tests

```bash
dotnet test
```

---

## 👨‍💻 Author

Tiago Ávila Saldanha

.NET Full Stack Developer

---

## 📄 License


This project is for educational and portfolio purposes.

