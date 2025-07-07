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
2. Restore required NuGet packages:
   ```sh
   dotnet restore
   ```

## Running

To start the API project:

```sh
cd src/IdleOrderService.Api
 dotnet run
```

The API will run by default at `http://localhost:5000`.

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
