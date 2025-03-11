# AuraDecor

AuraDecor is a comprehensive furniture management system with a modern web API backend built on ASP.NET Core 8.0. The application allows users to browse furniture items, manage their shopping cart, and place orders, while administrators can manage inventory, users, and special offers.

## Project Architecture

The solution follows the Clean Architecture pattern with separate layers for clear separation of concerns:

- **AuraDecor.APIs**: API controllers and presentation layer
- **AuraDecor.Core**: Domain entities, interfaces, and business rules
- **AuraDecor.Repository**: Data access and Entity Framework Core implementation
- **AuraDecor.Services**: Business logic implementation

## Technologies Used

- **ASP.NET Core 8.0**: Backend framework
- **Entity Framework Core**: ORM for database operations
- **ASP.NET Core Identity**: Authentication and authorization
- **JWT Authentication**: Token-based authentication
- **Google Authentication**: External authentication provider
- **Swagger/Scalar**: API documentation
- **AutoMapper**: Object mapping

## Features

- User authentication and authorization with role-based access control
- Comprehensive furniture catalog with search and filtering
- Shopping cart functionality
- Special offers and discounts
- User profile and address management
- Admin panel for user management and inventory control

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or later / Visual Studio Code

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Aura-Decor/Back-end.git
   ```

2. Navigate to the project folder and restore dependencies:
   ```bash
   cd AuraDecor
   dotnet restore
   ```

3. Update the connection string in `appsettings.json` in the AuraDecor.APIs project.

4. Apply migrations to create the database:
   ```bash
   dotnet ef database update --project AuraDecor.Repository --startup-project AuraDecor.APIs
   ```

5. Run the application:
   ```bash
   dotnet run --project AuraDecor.APIs
   ```

## API Endpoints

The API provides the following key endpoints:

### Authentication
- POST `/api/account/login` - User login with credentials
- POST `/api/account/register` - Register a new user
- GET `/api/account/google-login` - Initiate Google authentication
- GET `/api/account/google-response` - Handle Google authentication callback
- GET `/api/account/emailexists?email={email}` - Check if email already exists
- PUT `/api/account/updatepassword` - Update user password (requires authentication)

### User Profile Management
- GET `/api/account` - Get current user information (requires authentication)
- PUT `/api/account/update` - Update user profile (requires authentication)
- GET `/api/account/address` - Get user's address (requires authentication)
- PUT `/api/account/address` - Update or add user's address (requires authentication)

### Furniture Management
- GET `/api/furniture` - Get all furniture with filtering, sorting and pagination:
  - `brandId` - Filter by brand
  - `categoryId` - Filter by category
  - `sort` - Sort options (name, price, etc.)
  - `pageIndex` - Page number
  - `pageSize` - Items per page
  - `search` - Search by name
- GET `/api/furniture/{id}` - Get a specific furniture item by ID
- POST `/api/furniture` - Add a new furniture item (Admin)
- PUT `/api/furniture/{id}` - Update a furniture item (Admin)
- DELETE `/api/furniture/{id}` - Delete a furniture item (Admin)

### Offers & Discounts
- POST `/api/furniture/{id}/offers` - Apply special offer to a furniture item (Admin)
- DELETE `/api/furniture/{id}/offers` - Remove offer from a furniture item (Admin)
- GET `/api/furniture/offers/active` - Get all furniture with active offers
- POST `/api/furniture/offers/update-status` - Update status of all offers

### Admin Operations
- GET `/api/admin` - Get current admin information (Admin only)
- GET `/api/admin/users` - Get all system users (Admin only)
- POST `/api/admin/create-role` - Create a new system role (Admin only)
- POST `/api/admin/assign-role` - Assign role to a user (Admin only)

## Documentation

API documentation is available through Swagger UI when running the application in development mode. Access it at `/swagger`.


## License

This project is licensed under the MIT License - see the LICENSE file for details.
- Copyright 2025 © - MIT License
- [Mohammed Mostafa](https://github.com/mo7ammedd)
- [Hasnaa Abdelrahman](https://github.com/HAsNaaAbdelRahman)
- [Albassel Abobakr](https://github.com/Bassel-11)

