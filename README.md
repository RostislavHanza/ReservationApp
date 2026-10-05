# Reservation App

The application allows users to create, edit, view and delete reservations for event.

## Technologies

### Backend
- ASP.NET Core (.NET 9)
- Entity Framework Core
- SQLite
- REST API
- Dependency Injection / IoC

### Frontend
- React
- TypeScript
- Vite
- Material UI (MUI)
- React Hook Form

## Features

- Create a new reservation
- Edit an existing reservation using a reservation code
- Delete an existing reservation
- Reservation confirmation with generated reservation code
- Maximum 10 tickets per reservation
- Maximum event capacity of 100 tickets
- Frontend and backend validation
- Reservation operation journal / audit log
- Audit information includes:
  - operation type
  - reservation code
  - timestamp
  - IP address
  - browser / User-Agent
  - previous reservation state
  - new reservation state
- SQLite database
- Automatic database migration on backend startup

## Requirements

To run the application, the following software is required:

- .NET 9 SDK
- Node.js
- npm

## Configuration

### Backend

Backend configuration is located in:

```text
ReservationApp.Api/appsettings.json
```

Example configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=reservations.db"
  },
  "Event": {
    "Capacity": 100
  },
  "FrontendUrl": "http://localhost:5173"
}
```

### Frontend

Frontend API configuration is located in:

```text
reservationapp.frontend/.env
```

Example:

```env
VITE_API_URL=http://localhost:5094
```

## Running the Application

### Option 1 - start.bat

From the root directory, run:

```text
start.bat
```

The script starts both the backend and frontend.

### Option 2 - Manual Start

#### Backend

Open a terminal in:

```text
ReservationApp.Api
```

Run:

```bash
dotnet restore
dotnet run
```

The backend runs on:

```text
http://localhost:5094
```

#### Frontend

Open another terminal in:

```text
reservationapp.frontend
```

Run:

```bash
npm install
npm run dev
```

The frontend runs on:

```text
http://localhost:5173
```

Open the frontend URL in a browser.

## Database

The application uses SQLite.

The database file is:

```text
reservations.db
```

EF Core migrations are applied automatically when the backend starts:

```csharp
db.Database.Migrate();
```

If the database file does not exist, it is created automatically.

The database contains reservation data and journal / audit records.

## Reservation Rules

- Email is required and validated
- Phone number is required
- Phone number must contain exactly 9 digits
- Ticket count must be between 1 and 10
- Total event capacity is 100 tickets
- A reservation code is generated after successful reservation creation

## Reservation Journal

API operations are recorded in the reservation journal.

The journal stores:

- reservation ID
- reservation code
- operation type
- timestamp
- IP address
- browser / User-Agent
- old reservation state
- new reservation state

Reservation states are stored as JSON.

This makes it possible to track when and how a reservation was created, modified or deleted.

## API

Main reservation endpoints:

```text
POST   /api/reservations/create
GET    /api/reservations/get/{code}
PUT    /api/reservations/change
DELETE /api/reservations/delete/{code}
```

## Notes

- The frontend and backend have separate configuration files.
