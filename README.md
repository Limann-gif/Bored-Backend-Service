# BoredWeb

A REST API backend for a social activity platform that connects users to discover, create, and book group activities.

## Tech Stack

- **Framework:** ASP.NET Core 8.0
- **Language:** C#
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core 8.0 (Npgsql provider)
- **Authentication:** JWT Bearer tokens

## Features

- **User Management** — Registration, login, and user profiles with roles (user/admin)
- **Activity Listings** — Full CRUD for activities with categories, pricing, capacity, and group size settings
- **Bookings** — Individual and group bookings with support for multiple participants
- **Payments** — Transaction tracking with payment status management (pending/success/failed)
- **Complaints** — Support ticket system with status tracking (open/resolved/dismissed)
- **Activity History** — Track user participation across past activities
- **Matching** — Dedicated matching system for connecting users to activities

## Project Structure

```
BoredWeb/
├── Controllers/          # API route handlers
├── Models/               # Entity models and DTOs
├── Services/             # Business logic layer
├── Repositories/         # Data access layer
├── Data/                 # EF Core DbContext and factory
├── Migrations/           # Database migration files
├── Program.cs            # App startup and DI configuration
└── appsettings.json      # App configuration
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/user/signup` | Register a new user |
| POST | `/api/user/login` | Login and receive a JWT token |
| GET | `/api/user/getUser/{id}` | Get user details by ID |
| GET/POST/PUT/DELETE | `/api/activities/...` | Activity CRUD and booking |
| GET | `/api/matches/...` | Activity matching |

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### Setup

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd BoredWeb
   ```

2. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=bored;Username=your_user;Password=your_password"
   }
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at `http://localhost:5000` (or the configured port).

## Configuration

Key settings in `appsettings.json`:

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Key` | Secret key for signing JWT tokens |
| `Jwt:Issuer` | JWT issuer (`BoredWeb`) |
| `Jwt:Audience` | JWT audience (`MyWebAppUsers`) |

> JWT tokens expire after 10 minutes by default.

## Frontend

The API is configured to accept requests from `http://localhost:5173` (e.g., a Vite-based frontend). Update the CORS policy in `Program.cs` to match your frontend's origin.

## Database Schema

Core entities:

- **User** — Profiles with bio, occupation, location, and phone
- **Activity** — Listings with price, capacity, group size limits, and status (`forming` → `confirmed` → `completed` / `cancelled`)
- **ActivityBookingOrder** — Booking records linking users to activities with participant arrays
- **Transaction** — Payment records tied 1-to-1 with bookings
- **Complaint** — Support tickets with category and resolution tracking
- **GroupManagement** — Group coordination for multi-participant bookings
