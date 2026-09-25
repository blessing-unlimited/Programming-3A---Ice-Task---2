# Contract Central

Contract Central is an ASP.NET Core MVC application for managing clients and their contracts. It was built with .NET 8 and Entity Framework Core.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An IDE such as Visual Studio or Visual Studio Code (optional)

No separate database server is required. The application currently uses EF Core's in-memory database.

## Run the application

1. Clone or download this repository.
2. Open a terminal in the repository root (the directory containing `ContractCentral.sln`).
3. Restore dependencies and start the web project:

   ```powershell
   dotnet restore ContractCentral.sln
   dotnet run --project IceTask_Two/ContractCentral.csproj
   ```

4. Open the URL printed in the terminal. With the included launch settings, the usual addresses are:
   - HTTP: http://localhost:5183
   - HTTPS: https://localhost:7078

To build without running the application:

```powershell
dotnet build ContractCentral.sln
```

In Visual Studio, open `ContractCentral.sln`, set `IceTask_Two` as the startup project if needed, and run the `https` or `http` profile.

## Features

- Create, view, edit, delete, and search clients by name, contact person, or email.
- Create contracts for existing clients and view contract details.
- Search contracts by title or description, and filter by status or client.
- Change contract status according to the workflow rules.
- Automatically mark overdue Active and OnHold contracts as Expired. The background monitor checks once per minute.

### Contract status workflow

New contracts start as **Draft**. Allowed changes are:

- Draft → Active or OnHold
- Active → OnHold or Expired
- OnHold → Draft, Active, or Expired
- Expired → Draft (for a new cycle or renewal)

## Data storage

The application registers `AppDbContext` with EF Core's in-memory provider. Data exists only for the lifetime of the running application and is cleared when it stops. This setup is useful for a demo or coursework project, but data will not persist across restarts and is not shared with other app instances.

## Project layout

- `IceTask_Two/Controllers/` — MVC request handlers for clients, contracts, and home pages.
- `IceTask_Two/Models/` — entities, the database context, repository, and contract workflow/status definitions.
- `IceTask_Two/Services/` — background contract expiry monitor.
- `IceTask_Two/Views/` — Razor pages.
- `IceTask_Two/wwwroot/` — static CSS, JavaScript, and library assets.
