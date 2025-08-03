# Contract Management System

A multi-tenant, microservices-based Contract Management System built with React, ASP.NET Core, PostgreSQL, and RabbitMQ.

## Architecture

- **Frontend**: React + TypeScript + Vite
- **Backend**: ASP.NET Core microservices
- **Database**: PostgreSQL with Entity Framework Core
- **Messaging**: RabbitMQ
- **Development**: Docker Compose

## Quick Start

1. **Start infrastructure**:
   ```bash
   cd infrastructure
   docker-compose up -d
   ```

2. **Run migrations** (in each service directory):
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Start services** (separate terminals):
   ```bash
   # Identity Service
   cd src/services/IdentityService/IdentityService
   dotnet run

   # Contract Service  
   cd src/services/ContractService/ContractService
   dotnet run

   # Document Service
   cd src/services/DocumentService/DocumentService
   dotnet run
   ```

4. **Start frontend**:
   ```bash
   cd src/frontend
   npm install
   npm run dev
   ```

## Development

Open `contract-management-system.code-workspace` in VS Code for the best development experience.

## Services

- **Identity Service** (Port 5001): Authentication & tenant management
- **Contract Service** (Port 5002): Core contract management
- **Document Service** (Port 5003): Document storage & processing
- **Notification Service** (Port 5004): Email/SMS notifications
- **Audit Service** (Port 5005): Audit logging

## API Documentation

- Swagger UI available at `http://localhost:{port}/swagger` for each service
- RabbitMQ Management: `http://localhost:15672` (cms_user/cms_password)
