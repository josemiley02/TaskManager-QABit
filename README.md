# TaskManager-QABit
Task Manager. CRUD operations with Tasks, filters by search. STACK: C# and PostgreSQL


# Arquitectura de Archivos 

TaskManager-QABit/
│
├── src/
│   ├── TaskManager.Domain/
│   │   ├── Entities/
│   │   └── Enums/
│   │
│   ├── TaskManager.Application/
│   │   ├── Interfaces/
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Services/
│   │
│   ├── TaskManager.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Configurations/
│   │   │   └── Migrations/
│   │   └── DependencyInjection.cs
│   │
│   └── TaskManager.Api/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Extensions/
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   └── TaskManager.Tests/
│
├── TaskManager-QABit.sln
├── Directory.Build.props
└── README.md