# Health App

A simple ASP.NET Core Razor Pages web application for managing medical appointments.

Users can register and log in, then use the app depending on their role:

- Patients can view their appointments and book new ones.
- Doctors can see their schedule in a calendar view.
- Admins can manage user roles and appointments.

The project uses ASP.NET Core Identity for authentication and SQLite with Entity Framework Core for data storage.

## Requirements

- .NET 8 SDK
- Git

## Installation

Clone the repository:

```bash
git clone <repository-url>
cd epita-2025-health-app-78774-78775
```

Restore the dependencies:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

Run the Razor web app:

```bash
dotnet run --project HealthApp.Razor
```

Then open the app in your browser:

```text
http://localhost:5248
```

or, if HTTPS is enabled:

```text
https://localhost:7024
```

## Database

The application uses a local SQLite database:

```text
HealthApp.Razor/app.db
```

If you need to recreate or update the database, run:

```bash
dotnet ef database update --project HealthApp.Razor
```

If the `dotnet ef` command is not available, install it first:

```bash
dotnet tool install --global dotnet-ef
```

## Project Structure

```text
HealthApp.Razor/    Main Razor Pages web application
HealthApp.Domain/   Domain project
```

## Technologies

- ASP.NET Core Razor Pages
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
