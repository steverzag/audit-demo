# AuditDemo

This project is a modular .NET solution focused on implementing auditing capabilities using Entity Framework Core, Interceptors, and Reflection. It provides a clean and extensible architecture for tracking changes to entities, capturing audit logs automatically, and configuring audit behavior dynamically. The solution serves as a practical template for integrating robust auditing into .NET applications, making it ideal for learning, prototyping, or adopting modern audit patterns in enterprise environments.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server Database

## Installation
1. Clone this repository:
   ```sh
   git clone https://github.com/steverzag/audit-demo.git
   cd <repository-folder>
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Build the application:
   ```sh
   dotnet build
   ```
4. Apply Database Migrations:
  ```sh
  dotnet ef database update --project ./AuditDemo.API/AuditDemo.API.csproj
  ```
  Make sure you have the EF Core tools installed globally:
  ```sh
  dotnet tool install --global dotnet-ef
  ```


## Running the Application
To run the application locally:
   ```sh
   dotnet run --project ./AuditDemo.API/AuditDemo.API.csproj
   ```

Or to run the application using [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
   ```
   dotnet run --project ./AuditDemo.AppHost/AuditDemo.AppHost.csproj
   ```

By default, the application will be available at `http://localhost:5000` (or `https://localhost:5001` for HTTPS).

## Usage
The application exposes endpoints for creating and updating users and tasks, with all changes automatically tracked and logged for auditing purposes. Each operation generates audit records that capture who made the change, what was changed, and when it occurred. 

> Refer to the included [AuditDemo.API.http](AuditDemo.API/AuditDemo.API.http) file for more details.

## Audit Implementation

To enable entity auditing in this project, follow these steps:

1. **Create an Audited Entity Class:**  
   Extend the entity you want to audit by creating a new class that inherits from it.

2. **Define the Auditing Entity:**  
   Create an audit entity class that inherits from [`AuditingEntity<T>`](AuditDemo.API/Data/Models/Audit/AuditingEntity.cs), using your audited entity as the generic type parameter.

3. **Include the Auditing Entity in the DbContext**
   Add a `DbSet<YourAuditEntity>` property to your `DbContext` for the audit entity. For example:
   ```csharp
   public DbSet<UserAudit> UserAudits { get; set; }
   public DbSet<TaskAudit> TaskAudits { get; set; }
   ```
   This ensures that audit records are tracked and persisted in the database.

For reference implementations, see [UserAudit.cs](AuditDemo.API/Data/Models/Audit/UserAudit.cs) and [TaskAudit.cs](AuditDemo.API/Data/Models/Audit/TaskAudit.cs). These files demonstrate how to set up auditing for different entities within the solution.

> **Note:** Do not pass the entity to be audited directly to `AuditingEntity<T>`. Doing so will result in an entity configuration exception. Instead, create a separate audit entity that inherits from `AuditingEntity<T>` and configure it accordingly.

    