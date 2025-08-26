# UserManagerDemo
 
Watch the demo
https://github.com/user-attachments/assets/29180331-1053-4345-8834-d3092001b620


A demo User Management application using **ASP.NET Core 9** (Clean Architecture) for the backend and **Angular 20** for the frontend. Supports:

- User CRUD with **Identity** and **Profile**  
- JWT Authentication & Refresh Tokens  
- FluentValidation & FluentResults  
- Pagination & Filtering  
- Angular Material + Bootstrap UI  
- Standalone Angular Components  

---

## Table of Contents

- [Tech Stack](#tech-stack)  
- [Backend Setup](#backend-setup)  
- [Frontend Setup](#frontend-setup)  
- [API Endpoints](#api-endpoints)  
- [Authentication](#authentication)  
- [Pagination & Filtering](#pagination--filtering)  
- [Project Structure](#project-structure)  
- [Future Improvements](#future-improvements)  

---

## Tech Stack

- **Backend**: ASP.NET Core 9, Entity Framework Core, Identity, FluentValidation, AutoMapper  
- **Frontend**: Angular 20, Angular Material, Bootstrap 5, RxJS  
- **Database**: SQL Server (can be configured via `appsettings.json`)  

---

## Backend Setup

1. Clone the repo:

```bash
git clone https://github.com/your-repo/UserManagerDemo.git
cd asp-net/UserManagerDemo
```
2. Configure the database in appsettings.json:

```json
"ConnectionStrings": {
	"DefaultConnection": "Server=localhost;Database=YourAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. Apply migrations and create database

```bash
dotnet ef database update -p UserManagerDemo.Infrastructure -s UserManagerDemo.Api
```

4. Run the API:
```bash
cd UserManagerDemo.Api
dotnet run
```

## Frontend Setup

1. Navigate to the Angular project:

```bash
cd angular/user-manager-demo
```
2. Install dependencies:
```bash
npm install
```
3. Run the Angular app:
```bash
ng serve -o
```
## API Endpoints

Authentication

| Method | Route                 | Description                       |
|--------|-----------------------|-----------------------------------|
| POST   | /api/Auth/Register    | Register a new user               |
| POST   | /api/Auth/Login       | Login, return JWT & refresh token |
| POST   | /api/Auth/Refresh     | Refresh access token              |
| POST   | /api/Auth/Logout      | Logout & revoke refresh tokens    |
| Get    | /api/Auth/CurrentUser | Get current user Login            |


User Management

| Method | Route                 | Description               |
| ------ | --------------------- | ------------------------- |
| GET    | /api/User/GetAll      | Get all users             |
| GET    | /api/User/GetPagging  | Get users with pagination |
| PUT    | /api/User/Update/{id} | Update user profile       |
| DELETE | /api/User/Delete/{id} | Delete user               |

Authentication

- JWT access token stored in HttpOnly Cookie (jwt)
- Refresh token stored in HttpOnly Cookie (refreshToken)
- RememberMe option extends token lifetime

Claims

- nameidentifier: UserId
- emailaddress: User Email
- Frontend uses AuthService to manage login state & token refresh.

Pagination & Filtering

Backend: ```PagedResult<T>``` generic class with:
- ```Items```, ```TotalCount```, ```PageIndex```, ```PageSize```, ```TotalPages```
- ```HasPreviousPage```, ```HasNextPage```

Frontend: Angular Material ```MatTableDataSource``` with filtering via ```filterPredicate```.

Project Structure

Backend
```
UserManagerDemo/
├─ Api/               # ASP.NET Core API
│  ├─ Controllers/    # AuthController, UserController, BaseApiController
│  ├─ Attributes/     # Unit of work Attribute
│  ├─ Common/		  # Generic PagedResult
│  ├─ Extensions/     # ApplicationBuilderExtensions
│  ├─ Middlewares/    # Unit of Work Middleware
│  └─ Program.cs
├─ Application/
│  ├─ Users/          # DTOs, Validators, Mappings
│  ├─ Auth/           # DTOs, Validators
│  ├─ Common/         # Generic IRepository 
│  └─ Interface/	  # IJwtTokenGenerator
├─ Domain/            # Entities, Common
├─ Infrastructure/    # EF Core, Repositories, Services

 ```
 Frontend
 ```
 user-manager-demo/
 ├─ src/
 │  ├─ app/
 │  │  ├─ components/      # Standalone components
 │  │  ├─ services/        # AuthService, UserService
 │  │  ├─ models/          # User, AuthResponse, PagedResult
 │  │  ├─ app.route.ts
 │  │  ├─ app.ts
 │  │  ├─ app.css
 │  │  ├─ app.config.ts
 │  │  └─ app.html
 │  ├─ assets/
 │  ├─ environments/
 │  └─ styles.css
 ├─ angular.json
 ├─ package.json
 └─ tsconfig.json

 ```

Future Improvements

- Add role-based authorization
- Enhance UI/UX for user management
- Add server-side pagination & filtering
- Implement unit & integration tests
- Secure cookies with SameSite=Strict in production

Notes

- Backend uses FluentResults to wrap API responses consistently
- Frontend handles FluentResult and displays errors array when request fails
- Angular standalone components are used (no app.module.ts)
