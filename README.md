# IdleOrderService

IdleOrderService is a multi-layered .NET project that implements a clean architecture pattern with event-driven design. The project manages user registration and order processing with comprehensive unit testing coverage.

## 🏗️ Architecture

The project follows Clean Architecture principles with the following layers:

```
src/
  IdleOrderService.Api/           # REST API layer (Controllers, Program.cs)
  IdleOrderService.Application/   # Application services (Commands, Handlers, DTOs)
  IdleOrderService.Core/          # Core infrastructure (Mediator, Events, Interfaces)
  IdleOrderService.Domain/        # Domain models and business rules
  IdleOrderService.Infra/         # Infrastructure (Data access, Event buses, Decorators)
```

## 🧪 Testing & Code Coverage

The project includes comprehensive unit tests with **37.3% line coverage** and **33.8% branch coverage**.

### Test Structure
```
test/
  IdleOrderService.Test/
    ├── Application Layer Tests
    │   ├── RegisterUserCommandHandlerTests.cs
    │   └── UserRegisteredEventHandlerTests.cs
    ├── API Layer Tests
    │   └── UsersControllerTests.cs
    ├── Core Infrastructure Tests
    │   ├── MediatorTests.cs
    │   ├── InMemoryEventBusTests.cs
    │   └── KafkaEventBusTests.cs
    ├── Infrastructure Tests
    │   ├── EfEventStoreTests.cs
    │   ├── AppDbContextTests.cs
    │   └── OutboxEventTests.cs
    ├── Decorator Tests
    │   ├── LoggingEventHandlerDecoratorTests.cs
    │   └── RetryingEventHandlerDecoratorTests.cs
    ├── Middleware Tests
    │   ├── LoggingMiddlewareTests.cs
    │   └── MetricsMiddlewareTests.cs
    └── DI Configuration Tests
        └── ServiceCollectionExtensionsTests.cs
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Generate coverage report
reportgenerator -reports:./TestResults/*/coverage.cobertura.xml -targetdir:./TestResults/coverage-report -reporttypes:Html
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Docker & Docker Compose (for full stack)
- PostgreSQL (if running locally)
- Kafka (if running locally)

### Installation

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd IdleOrderService
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## 🐳 Running with Docker Compose (Recommended)

Start the complete stack with a single command:

```bash
docker-compose up -d --build
```

This will start:
- **IdleOrderService.Api** (http://localhost:5000)
- **PostgreSQL** (Database: `idle_order_db`)
- **Kafka** (localhost:9092)
- **Zookeeper**

### API Endpoints

Once running, you can access:
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

### Example API Usage

```bash
# Register a new user
curl -X POST "http://localhost:5000/api/users/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "fullName": "John Doe"
  }'
```

## 🏃‍♂️ Running Locally

### Option 1: Full Local Setup

1. Start dependencies with Docker:
   ```bash
   docker-compose up -d postgres kafka zookeeper
   ```

2. Run the API locally:
   ```bash
   cd src/IdleOrderService.Api
   dotnet run
   ```

### Option 2: In-Memory Database (for development)

Modify `Program.cs` to use in-memory database for faster development:

```csharp
services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("IdleOrderDb");
});
```

## 🏛️ Architecture Details

### Event-Driven Architecture

The project implements an event-driven architecture with:

- **Event Bus**: `IEventBus` interface with `InMemoryEventBus` and `KafkaEventBus` implementations
- **Event Store**: `EfEventStore` for event persistence using Entity Framework
- **Outbox Pattern**: `OutboxDispatcher` for reliable event publishing
- **Decorators**: Logging and retry decorators for event handlers

### Mediator Pattern

Uses a custom mediator implementation for command/query handling:

```csharp
// Register a command handler
services.AddScoped<IRequestHandler<RegisterUserCommand, UserDto>, RegisterUserCommandHandler>();

// Use in controller
var result = await _mediator.Send(command);
```

### Middleware Pipeline

Custom middleware for cross-cutting concerns:

```csharp
services.AddScoped(typeof(IExecutionMiddleware<,>), typeof(LoggingMiddleware<,>));
services.AddScoped(typeof(IExecutionMiddleware<,>), typeof(MetricsMiddleware<,>));
```

## 📊 Database Schema

The project uses Entity Framework Core with PostgreSQL:

### Outbox Events Table
```sql
CREATE TABLE outbox_events (
    Id UUID PRIMARY KEY,
    Type TEXT NOT NULL,
    Payload TEXT NOT NULL,
    OccurredAt TIMESTAMP WITH TIME ZONE NOT NULL,
    Processed BOOLEAN DEFAULT FALSE,
    ProcessedAt TIMESTAMP WITH TIME ZONE,
    Priority INTEGER DEFAULT 1
);
```

## 🔧 Configuration

### Environment Variables
- `ASPNETCORE_URLS`: API listening URLs (default: http://localhost:5000)
- `ConnectionStrings__Default`: PostgreSQL connection string
- `Kafka__BootstrapServers`: Kafka broker addresses

### App Settings
Key configuration in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=idle_order_db;Username=myuser;Password=mysecretpassword"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  }
}
```

## 🧪 Testing Strategy

### Test Categories
1. **Unit Tests**: Individual component testing with mocked dependencies
2. **Integration Tests**: Database and external service integration
3. **API Tests**: End-to-end API endpoint testing

### Test Coverage Goals
- **Line Coverage**: >35% (Current: 37.3%)
- **Branch Coverage**: >30% (Current: 33.8%)
- **Critical Path Coverage**: 100%

### Test Dependencies
- **xUnit**: Testing framework
- **Moq**: Mocking framework
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for testing
- **coverlet.collector**: Code coverage collection

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for new functionality
4. Ensure all tests pass (`dotnet test`)
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

### Development Guidelines
- Follow Clean Architecture principles
- Write unit tests for new features
- Maintain test coverage above 35%
- Use meaningful commit messages
- Update documentation for API changes

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Troubleshooting

### Common Issues

**Docker Compose fails to start:**
```bash
# Clean up containers and volumes
docker-compose down -v
docker system prune -f
docker-compose up -d --build
```

**Database connection issues:**
```bash
# Check if PostgreSQL is running
docker ps | grep postgres

# Check logs
docker-compose logs postgres
```

**Kafka connection issues:**
```bash
# Check if Kafka is running
docker ps | grep kafka

# Check Kafka logs
docker-compose logs kafka
```

**Test failures:**
```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```
