# IdleOrderService

IdleOrderService is a multi-layered .NET project that manages order processing and user operations. The project consists of API, Application, Domain, Core, and Infrastructure layers.

## Folder Structure

```
src/
  IdleOrderService.Api/           # REST API layer
  IdleOrderService.Application/   # Application services and business logic
  IdleOrderService.Core/          # Core infrastructure, mediator, and event structures
  IdleOrderService.Domain/        # Domain models and business rules
  IdleOrderService.Infra/         # Infrastructure, data access, and external service integrations
```

## Installation

1. Clone the repository:
   ```sh
   git clone <repo-url>
   cd IdleOrderService
   ```
2. Restore required NuGet packages (if you want to run locally):
   ```sh
   dotnet restore
   ```

## Running with Docker Compose

You can run the API, PostgreSQL, Kafka, and Zookeeper services together using Docker Compose:

```sh
docker-compose up -d --build
```

This will start:
- **IdleOrderService.Api** (on port 5000)
- **PostgreSQL** (Database: `idle_order_db`, User: `myuser`, Password: `mysecretpassword`)
- **Kafka**
- **Zookeeper**

> The API will be accessible at `http://localhost:5000`.
> The API container uses an internal connection string to reach PostgreSQL (`Host=postgres;...`).
> If you want to run the API locally (outside Docker), make sure your connection string in `appsettings.json` uses `Host=localhost`.
> Kafka is accessible as `kafka:9092` from within containers, and as `localhost:9092` from your host.

### Notes on Docker Build
- The Dockerfile for the API is located at `src/IdleOrderService.Api/Dockerfile`.
- The build context in `docker-compose.yml` is set to the repository root to include the solution and all projects.
- Environment variables for connection strings and Kafka are set in the compose file and override appsettings.

## Running the API Locally (without Docker Compose)

If you prefer to run the API on your host machine:

1. Make sure PostgreSQL, Kafka, and Zookeeper are running (can be started with Docker Compose).
2. Use the default connection string in `appsettings.json` (Host=localhost).
3. Start the API:
   ```sh
   cd src/IdleOrderService.Api
   dotnet run
   ```
4. The API will run by default at `http://localhost:5000`.

## About the Layers

- **Api**: Handles HTTP requests; controllers are located here.
- **Application**: Contains commands, handlers, and DTOs.
- **Core**: Shared infrastructure, event, and mediator structures.
- **Domain**: Core domain models and business rules.
- **Infra**: Data access, migration, and external service integrations.

## Contributing

To contribute, please create a fork and submit a pull request.

## License

This project is licensed under the MIT License.
