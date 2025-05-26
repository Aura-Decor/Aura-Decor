# AuraDecor
- 
AuraDecor is a comprehensive furniture management system with a modern web API backend built on ASP.NET Core 8.0. The application allows users to browse furniture items, manage their shopping cart, and place orders, while administrators can manage inventory, users, and special offers.

## Features Completed ✅

- ✅ User Authentication & Authorization (JWT + Google OAuth)
- ✅ Furniture Catalog Management
- ✅ Shopping Cart System
- ✅ Order Management
- ✅ Special Offers & Discounts
- ✅ Notification System
- ✅ Rate Limiting
- ✅ Email Services (OTP, Notifications)

## to do 
- Coupon Module
- Payment Module
- Refresh Token
- Recommendation Module
- Unit Testing

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
- **Redis**: Distributed caching and session storage
- **Swagger/Scalar**: API documentation
- **AutoMapper**: Object mapping
- **MailKit**: Email sending functionality

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
- POST `/api/account/forgot-password` - Initiate password reset process
- POST `/api/account/verify-otp` - Verify one-time password for password reset
- POST `/api/account/reset-password` - Complete password reset with token

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

### Cart Management
- GET `/api/cart` - Get user's shopping cart (requires authentication)
- POST `/api/cart` - Add item to shopping cart (requires authentication)
- DELETE `/api/cart/{id}` - Remove item from cart (requires authentication)
- PUT `/api/cart/{id}` - Update cart item quantity (requires authentication)

### Notification System
#### User Notification Endpoints (Requires Authentication)
- GET `/api/notification` - Get paginated user notifications
  - `page` - Page number (default: 1)
  - `pageSize` - Items per page (default: 10)
- GET `/api/notification/unread` - Get all unread notifications for the current user
- GET `/api/notification/summary` - Get notification summary with unread count
- PUT `/api/notification/{id}/mark-read` - Mark a specific notification as read
- PUT `/api/notification/mark-all-read` - Mark all notifications as read for the current user
- DELETE `/api/notification/{id}` - Delete a specific notification
- DELETE `/api/notification/all` - Delete all notifications for the current user

#### Notification Preferences (Requires Authentication)
- GET `/api/notification/preferences` - Get user notification preferences
- PUT `/api/notification/preferences` - Update notification preferences
  ```json
  {
    "emailNotifications": true,
    "orderUpdates": true,
    "promotionalOffers": false,
    "systemAlerts": true,
    "cartReminders": true
  }
  ```

#### Admin Notification Endpoints (Admin Only)
- POST `/api/notification/admin/create` - Create notification for a specific user
  ```json
  {
    "userId": "user-guid-id",
    "title": "Notification Title",
    "message": "Notification message content",
    "type": 0
  }
  ```
- POST `/api/notification/admin/bulk` - Send bulk notifications
  ```json
  {
    "title": "System Announcement",
    "message": "Important system message",
    "type": 6,
    "userIds": ["user1-id", "user2-id"] // null for all users
  }
  ```

#### Notification Types
- `0` - Info
- `1` - Success  
- `2` - Warning
- `3` - Error
- `4` - OrderUpdate
- `5` - PromotionalOffer
- `6` - SystemAlert
- `7` - CartReminder
- `8` - WelcomeMessage

#### Automatic Notifications
The system automatically sends notifications for:
- **Welcome messages** when users register
- **Order status updates** when order status changes
- **Cart reminders** for abandoned carts
- **Promotional offers** when new deals are available
- **System alerts** for important announcements

#### Email Integration
- Notifications can be sent via email based on user preferences
- Users can control which notification types trigger emails
- Styled email templates with company branding
- OTP verification emails for password reset

## Documentation

API documentation is available through Swagger UI when running the application in development mode. Access it at `/swagger`.


## License

This project is licensed under the MIT License - see the LICENSE file for details.
- Copyright 2025 © - MIT License
- [Mohammed Mostafa](https://github.com/mo7ammedd)
- [Hasnaa Abdelrahman](https://github.com/HAsNaaAbdelRahman)
- [Albassel Abobakr](https://github.com/Bassel-11)

