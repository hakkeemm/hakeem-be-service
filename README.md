# Hakeem Backend (Phase 1)

This project contains the backend for Hakeem (حكيم), built with Clean Architecture, ASP.NET Core Web API, Entity Framework Core, and ASP.NET Core Identity. 

## Project Structure
- **Hakeem.Domain**: Entities, interfaces, enums.
- **Hakeem.Application**: Business logic, DTOs, Validation, Services.
- **Hakeem.Infrastructure**: EF Core DbContext, Migrations, Repositories, Identity, Email.
- **Hakeem.API**: ASP.NET Core Web API, Controllers, Middleware.

## Setup Instructions

### 1. Configure the Database
The project uses PostgreSQL. Update your connection string in `src/Hakeem.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=HakeemDb;Username=postgres;Password=postgres"
}
```

### 2. Configure JWT and Email Settings
Update `appsettings.json` (or use user-secrets) to provide valid JWT settings and SMTP configuration (e.g. your Gmail app password):
```json
"JwtSettings": {
  "Key": "YourSuperSecretKeyHere...",
  "Issuer": "Hakeem",
  "Audience": "HakeemUsers"
},
"EmailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "your_email@gmail.com",
  "Password": "your_app_password",
  "FromAddress": "noreply@hakeem.app"
}
```

### 3. Apply Migrations
From the solution root (`e:\ZEMAM\HAKEEM`), run the following command to create the database and apply the initial schema:
```powershell
dotnet ef database update --project src\Hakeem.Infrastructure --startup-project src\Hakeem.API
```

### 4. Run the API
```powershell
cd src\Hakeem.API
dotnet run
```

Access the Swagger UI at `https://localhost:<port>/swagger` to test the API endpoints. You can register a patient, verify their email (the endpoint will log or throw depending on if you run the frontend), and log in to get a JWT bearer token. Use the "Authorize" button in Swagger to pass the token for protected endpoints.
