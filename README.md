# EliosCVBuilderService

A .NET 9.0 microservice for managing user CV/resumes, implementing Clean Architecture with CQRS patterns, PostgreSQL JSONB storage, and Kafka event-driven communication.

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 16+ (or Neon Serverless)
- Apache Kafka (optional for inter-service messaging)
- Redis (optional for caching)

### Build and Run

```bash
# Restore dependencies
dotnet restore CVBuilder.sln

# Build solution
dotnet build CVBuilder.sln

# Run API
cd CVBuilder
dotnet run

# API available at http://localhost:5002
# Swagger UI at http://localhost:5002/swagger
```

### Run with Docker

```bash
# Build Docker image
docker build -t cvbuilder-service .

# Run container
docker run -p 80:80 cvbuilder-service

# Health check: http://localhost:80/health
```

### Run Tests

```bash
# Run all tests
dotnet test CVBuilder.Test/CVBuilder.Test.csproj

# Run with coverage
dotnet test CVBuilder.Test/CVBuilder.Test.csproj /p:CollectCoverage=true
```

## Architecture Overview

```
CVBuilder.Web (Presentation)
    ↓
CVBuilder.Core (Business Logic)
    ↓
CVBuilder.Domain (Entities)
    ↑
CVBuilder.Infrastructure (Data/External Services)

CVBuilder.Contract (DTOs/Messages)
CVBuilder.Test (Unit Tests)
```

**Clean Architecture**: Strict dependency rules ensure inner layers have no knowledge of outer layers.

## Key Features

- **Flexible CV Storage**: PostgreSQL JSONB columns for dynamic resume structures
- **Event-Driven**: Kafka messaging for inter-service communication
- **CQRS Pattern**: Separate command and query handlers via MediatR
- **Soft Deletes**: All entities support soft delete with recovery
- **Template System**: Pre-built CV templates for users
- **Ownership Validation**: Users can only access their own CVs
- **Transaction Management**: Atomic operations across database and Kafka

## Technology Stack

- **.NET 9.0** - ASP.NET Core Web API
- **PostgreSQL** - Neon Serverless with JSONB support
- **Apache Kafka** - Event streaming platform
- **Entity Framework Core 9.0** - ORM with LINQ
- **MediatR** - CQRS implementation
- **xUnit + Moq** - Unit testing
- **Swagger/OpenAPI** - API documentation

## Project Structure

```
CVBuilder/                     # Presentation (API Controllers)
CVBuilder.Core/                # Business Logic (Handlers, Interfaces)
CVBuilder.Domain/              # Domain Entities
CVBuilder.Infrastructure/      # Data Access, Kafka, External Services
CVBuilder.Contract/            # DTOs, Commands, Queries
CVBuilder.Test/                # Unit Tests
docs/                          # Comprehensive documentation
```

## API Endpoints

### UserCvs (`/api/cvbuilder/usercvs`)

- `POST /api/cvbuilder/usercvs` - Create new CV
- `GET /api/cvbuilder/usercvs/{id}` - Get CV by ID
- `GET /api/cvbuilder/usercvs` - List user's CVs (paginated)
- `PUT /api/cvbuilder/usercvs/{id}` - Update CV
- `DELETE /api/cvbuilder/usercvs/{id}` - Soft delete CV

### CvTemplates (`/api/v1/cvtemplates`)

- `POST /api/v1/cvtemplates` - Create template
- `GET /api/v1/cvtemplates/{id}` - Get template by ID
- `GET /api/v1/cvtemplates` - List templates (paginated)
- `PUT /api/v1/cvtemplates/{id}` - Update template
- `DELETE /api/v1/cvtemplates/{id}` - Soft delete template

**Authentication**: All endpoints require `X-Auth-Request-User` header with user GUID.

## Configuration

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Database=CVBuilder;..."
  },
  "Kafka": {
    "BootstrapServers": "localhost:9094",
    "CurrentService": "cvbuilder",
    "SourceServices": ["user", "utility"]
  }
}
```

**Environment Variables** (recommended for production):
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `Kafka__BootstrapServers` - Kafka bootstrap servers

## Database Migrations

```bash
# Add migration
dotnet ef migrations add MigrationName \
  --project CVBuilder.Infrastructure \
  --startup-project CVBuilder

# Update database
dotnet ef database update \
  --project CVBuilder.Infrastructure \
  --startup-project CVBuilder

# Generate SQL script
dotnet ef migrations script \
  --project CVBuilder.Infrastructure \
  --startup-project CVBuilder
```

## Kafka Integration

**Topic Naming**: `{source-service}-{destination-service}-{entity-type}`

**Examples**:
- `cvbuilder-user-usercv` - CVBuilder → User service
- `user-cvbuilder-usercv` - User → CVBuilder service

**Event Types**: CREATE, UPDATE, DELETE, GET_ALL, GET_BY_ID

**Consumer Registration**: Dynamically registered based on `Kafka:SourceServices` configuration.

## Development Guidelines

### Adding a New Entity

1. Create domain model in `CVBuilder.Domain/Entities/`
2. Add DbSet to `CVBuilderDataContext`
3. Configure entity in `OnModelCreating`
4. Create migration: `dotnet ef migrations add AddNewEntity`
5. Update database: `dotnet ef database update`

### Adding a New API Endpoint

1. Create DTOs in `CVBuilder.Contract/TransferObjects/`
2. Create Command/Query in `CVBuilder.Contract/UseCases/`
3. Create Handler in `CVBuilder.Core/Handler/`
4. Add controller endpoint in `CVBuilder.Web/Controllers/`
5. Write unit tests in `CVBuilder.Test/`

See [docs/code-standards.md](docs/code-standards.md) for detailed coding standards.

## Common Pitfalls

1. **Missing Auth Header**: Always include `X-Auth-Request-User` header
2. **No Transactions**: Wrap mutations in transactions with rollback
3. **No Ownership Validation**: Always filter by `OwnerId == userId`
4. **Hardcoded Pagination**: Accept `pageNumber` and `pageSize` parameters
5. **Ignoring Soft Delete**: Use global query filter or explicit `IsDeleted = false`

## Documentation

Comprehensive documentation is available in the `docs/` directory:

- **[Project Overview & PDR](docs/project-overview-pdr.md)** - Business context, requirements, success metrics
- **[Codebase Summary](docs/codebase-summary.md)** - High-level structure, components, file locations
- **[Code Standards](docs/code-standards.md)** - Coding conventions, patterns, best practices
- **[System Architecture](docs/system-architecture.md)** - Clean Architecture, data flow, integrations
- **[CLAUDE.md](CLAUDE.md)** - AI assistant guidance for development

## Testing

**Unit Tests**: Located in `CVBuilder.Test/`

**Test Pattern** (AAA - Arrange, Act, Assert):
```csharp
[Fact]
public async Task Handle_ValidId_ReturnsUserCv()
{
    // Arrange
    var mockRepository = new Mock<IGenericRepository<UserCv>>();
    mockRepository.Setup(r => r.GetOneAsync(...)).ReturnsAsync(userCv);

    // Act
    var result = await handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.Equal(200, result.Status);
    Assert.NotNull(result.ResponseData);
}
```

**Coverage Target**: > 70% overall, 100% for critical paths (CRUD handlers)

## Deployment

### Docker

```bash
docker build -t cvbuilder-service .
docker run -p 80:80 \
  -e ConnectionStrings__DefaultConnection="Host=...;..." \
  -e Kafka__BootstrapServers="kafka:9092" \
  cvbuilder-service
```

### Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: cvbuilder-service
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: cvbuilder
        image: cvbuilder-service:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: cvbuilder-secrets
              key: database-connection
```

## Monitoring and Health

**Health Check**: Available at `/health` (Docker deployments)

**Recommended Monitoring**:
- Application Insights (Azure)
- Prometheus + Grafana
- Kafka Consumer Lag monitoring
- PostgreSQL query performance

## Security

**Current Implementation**:
- Header-based authentication (`X-Auth-Request-User`)
- Ownership validation on all CV operations
- EF Core parameterized queries (SQL injection prevention)
- Soft delete for data recovery
- PostgreSQL SSL connections

**Recommended Enhancements**:
- Move credentials to Azure Key Vault or environment variables
- Implement rate limiting and throttling
- Enable HTTPS enforcement
- Configure CORS for allowed origins
- Add API versioning

## Contributing

1. Create feature branch: `cvb-{ticket-number}-{description}`
2. Follow code standards in [docs/code-standards.md](docs/code-standards.md)
3. Write unit tests for all new features
4. Ensure all tests pass: `dotnet test`
5. Update documentation if needed
6. Submit pull request to `main` branch

**Branching Strategy**:
- Main branch: `main`
- Feature branches: `cvb-{ticket-number}-{description}`
- Current branch: `cvb-12-update-API`

## Troubleshooting

### Database Connection Issues

```bash
# Test connection string
dotnet ef database update --project CVBuilder.Infrastructure --startup-project CVBuilder
```

### Kafka Connection Issues

```bash
# Verify Kafka is running
docker ps | grep kafka

# Check consumer lag
kafka-consumer-groups.sh --bootstrap-server localhost:9094 --group cvbuilder-usercv-consumers --describe
```

### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

## License

[Your License Here]

## Contact

For questions or support, contact the development team or create an issue in the repository.

---

**Version**: 1.0.0
**Last Updated**: 2025-11-14
**Status**: MVP Complete - Ready for integration testing
