# AuraDecor

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

The API provides comprehensive endpoints organized by functionality. All endpoints return JSON responses and follow RESTful conventions.

### 🔐 Authentication Endpoints

#### User Authentication
- **POST** `/api/account/login` - User login with email and password
  ```json
  {
    "email": "user@example.com",
    "password": "Password123!"
  }
  ```
  **Response**: Returns JWT token and user details
  **Rate Limited**: 2 requests per 10 seconds

- **POST** `/api/account/register` - Register a new user account
  ```json
  {
    "displayName": "John Doe",
    "email": "user@example.com",
    "userName": "johndoe",
    "phoneNumber": "+1234567890",
    "password": "Password123!"
  }
  ```
  **Response**: Returns JWT token and user details
  **Rate Limited**: 2 requests per 10 seconds

#### External Authentication
- **GET** `/api/account/google-login` - Initiate Google OAuth authentication
  **Rate Limited**: 2 requests per 10 seconds

- **GET** `/api/account/google-response` - Handle Google OAuth callback
  **Response**: Returns JWT token and user details

- **GET** `/api/account/twitter-login` - Initiate Twitter OAuth authentication

- **GET** `/api/account/twitter-response` - Handle Twitter OAuth callback
  **Response**: Returns JWT token and user details

#### Password Management
- **POST** `/api/account/forgot-password` - Request password reset
  ```json
  {
    "email": "user@example.com"
  }
  ```
  **Response**: Sends OTP to email

- **POST** `/api/account/verify-otp` - Verify OTP for password reset
  ```json
  {
    "email": "user@example.com",
    "otp": "123456"
  }
  ```
  **Response**: Returns password reset token

- **POST** `/api/account/reset-password` - Complete password reset
  ```json
  {
    "email": "user@example.com",
    "token": "reset-token",
    "newPassword": "NewPassword123!"
  }
  ```

- **PUT** `/api/account/updatepassword` - Update current user's password
  ```json
  {
    "currentPassword": "OldPassword123!",
    "newPassword": "NewPassword123!"
  }
  ```
  **Authentication**: Required (JWT Bearer token)

#### Utility
- **GET** `/api/account/emailexists?email={email}` - Check if email is already registered
  **Response**: Boolean value

### 👤 User Profile Management

- **GET** `/api/account` - Get current authenticated user information
  **Authentication**: Required
  **Response**: User details with refreshed JWT token

- **PUT** `/api/account/update` - Update user profile information
  ```json
  {
    "displayName": "Updated Name",
    "phoneNumber": "+1234567890"
  }
  ```
  **Authentication**: Required
  **Response**: Updated user details with new JWT token

#### Address Management
- **GET** `/api/account/address` - Get user's saved address
  **Authentication**: Required
  **Response**: Address details or 404 if no address saved

- **PUT** `/api/account/address` - Add or update user's address
  ```json
  {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  }
  ```
  **Authentication**: Required

### 🪑 Furniture Management

#### Public Endpoints
- **GET** `/api/furniture` - Get all furniture with advanced filtering and pagination
  **Query Parameters**:
  - `brandId` (Guid) - Filter by specific brand
  - `categoryId` (Guid) - Filter by category
  - `sort` (string) - Sort options: "name", "priceAsc", "priceDesc", "newest"
  - `pageIndex` (int, default: 1) - Page number for pagination
  - `pageSize` (int, default: 6) - Number of items per page
  - `search` (string) - Search in furniture names and descriptions
  - `minPrice` (decimal) - Minimum price filter
  - `maxPrice` (decimal) - Maximum price filter
  
  **Response**: Paginated list with total count
  **Caching**: 5 minutes
  **Rate Limited**: 5 requests per 60 seconds

- **GET** `/api/furniture/{id}` - Get specific furniture item by ID
  **Response**: Detailed furniture information including images and specifications
  **Caching**: 5 minutes

#### Admin Only Endpoints
- **POST** `/api/furniture` - Add new furniture item
  **Content-Type**: multipart/form-data (supports file upload for images)
  ```json
  {
    "name": "Modern Sofa",
    "description": "Comfortable 3-seater sofa",
    "price": 899.99,
    "stock": 10,
    "brandId": "brand-guid",
    "categoryId": "category-guid"
  }
  ```
  **Authentication**: Admin role required

- **PUT** `/api/furniture/{id}` - Update existing furniture item
  **Authentication**: Admin role required

- **DELETE** `/api/furniture/{id}` - Delete furniture item
  **Authentication**: Admin role required
  **Response**: 204 No Content on success

### 🏷️ Offers & Discounts

#### Admin Endpoints
- **POST** `/api/furniture/{id}/offers` - Apply special offer to furniture item
  ```json
  {
    "discountPercentage": 20.0,
    "startDate": "2025-05-26T00:00:00Z",
    "endDate": "2025-06-26T23:59:59Z"
  }
  ```
  **Authentication**: Admin role required

- **DELETE** `/api/furniture/{id}/offers` - Remove offer from furniture item
  **Authentication**: Admin role required

- **POST** `/api/furniture/offers/update-status` - Update status of all offers (expire old ones)
  **Authentication**: Admin role required

#### Public Endpoints
- **GET** `/api/furniture/offers/active` - Get all furniture items with active offers
  **Response**: List of furniture with current discounts
  **Caching**: 5 minutes

### 🛒 Shopping Cart Management

All cart endpoints require authentication.

- **GET** `/api/cart` - Get user's current shopping cart
  **Response**: Cart with items, quantities, and total price

- **POST** `/api/cart/add` - Add item to shopping cart
  ```json
  {
    "furnitureId": "furniture-guid",
    "quantity": 2
  }
  ```

- **DELETE** `/api/cart/remove` - Remove item from cart
  ```json
  {
    "furnitureId": "furniture-guid"
  }
  ```

### 📦 Order Management

All order endpoints require authentication.

- **POST** `/api/order/CreatOrder` - Create new order from cart
  **Query Parameters**:
  - `UserId` (string) - User ID
  - `CartId` (Guid) - Cart ID to convert to order
  
  **Response**: Created order details

- **GET** `/api/order/{Id}` - Get order details by user ID
  **Response**: Order information with items and status

- **POST** `/api/order/CancelOrder` - Cancel an existing order
  **Query Parameters**:
  - `UserId` (string) - User ID
  - `OrderId` (Guid) - Order to cancel
  
  **Response**: Boolean indicating success

### 🔔 Notification System

#### User Notification Endpoints (Authentication Required)
- **GET** `/api/notification` - Get paginated user notifications
  **Query Parameters**:
  - `page` (int, default: 1) - Page number
  - `pageSize` (int, default: 10) - Items per page
  
  **Response**: Paginated list of notifications

- **GET** `/api/notification/unread` - Get all unread notifications
  **Response**: List of unread notifications only

- **GET** `/api/notification/summary` - Get notification summary with count
  **Response**: 
  ```json
  {
    "unreadCount": 5,
    "recentNotifications": [...]
  }
  ```

- **PUT** `/api/notification/{id}/mark-read` - Mark specific notification as read
  **Response**: 200 OK or 404 if notification not found

- **PUT** `/api/notification/mark-all-read` - Mark all notifications as read
  **Response**: 200 OK or 400 if no unread notifications

- **DELETE** `/api/notification/{id}` - Delete specific notification
  **Response**: 200 OK or 404 if notification not found

- **DELETE** `/api/notification/all` - Delete all user notifications
  **Response**: 200 OK or 400 if no notifications to delete

#### Notification Preferences (Authentication Required)
- **GET** `/api/notification/preferences` - Get user notification preferences
  **Response**: Current preference settings

- **PUT** `/api/notification/preferences` - Update notification preferences
  ```json
  {
    "emailNotifications": true,
    "orderUpdates": true,
    "promotionalOffers": false,
    "systemAlerts": true,
    "cartReminders": true
  }
  ```

#### Admin Notification Endpoints (Admin Role Required)
- **POST** `/api/notification/admin/create` - Create notification for specific user
  ```json
  {
    "userId": "user-guid-id",
    "title": "Important Notice",
    "message": "Your order has been shipped!",
    "type": 4,
    "relatedEntityId": "order-guid",
    "relatedEntityType": "Order"
  }
  ```

- **POST** `/api/notification/admin/bulk` - Send bulk notifications
  ```json
  {
    "title": "System Maintenance",
    "message": "Scheduled maintenance tonight from 2-4 AM",
    "type": 6,
    "userIds": ["user1-id", "user2-id"]
  }
  ```
  **Note**: Leave `userIds` null to send to all users

#### Notification Types Reference
- `0` - Info (General information)
- `1` - Success (Positive confirmations)
- `2` - Warning (Important alerts)
- `3` - Error (Error notifications)
- `4` - OrderUpdate (Order status changes)
- `5` - PromotionalOffer (Marketing offers)
- `6` - SystemAlert (System announcements)
- `7` - CartReminder (Cart abandonment reminders)
- `8` - WelcomeMessage (New user welcome)

#### Automatic Notifications
The system automatically generates notifications for:
- **User Registration**: Welcome message with account setup tips
- **Order Creation**: Order confirmation with details
- **Order Status Updates**: Shipping, delivery, cancellation notifications
- **Cart Abandonment**: Reminders for items left in cart (configurable timing)
- **Promotional Campaigns**: New offers and discounts (if user opted in)
- **System Maintenance**: Important system announcements

### 👨‍💼 Admin Management

All admin endpoints require Admin role authentication.

- **GET** `/api/admin/users` - Get list of all system users
  **Response**: Array of user objects with basic information

- **POST** `/api/admin/create-role` - Create new system role
  ```json
  "Manager"
  ```
  **Request Body**: Role name as string

- **POST** `/api/admin/assign-role` - Assign role to user
  ```json
  {
    "email": "user@example.com",
    "roleName": "Manager"
  }
  ```

### ⚠️ Error Handling

- **GET** `/errors/{code}` - Error page endpoint for HTTP status codes
  **Response**: Standardized error response format

#### Standard Error Response Format
```json
{
  "statusCode": 404,
  "message": "Resource not found",
  "details": "Additional error details when available"
}
```

#### Validation Error Response Format
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    "Email is required",
    "Password must be at least 8 characters"
  ]
}
```

### 🛡️ Security & Rate Limiting

#### Authentication
- **JWT Bearer Tokens**: Required for protected endpoints
- **Token Format**: `Authorization: Bearer {jwt-token}`
- **Token Expiration**: Configurable (typically 24 hours)

#### Rate Limiting
- **Login/Register**: 2 requests per 10 seconds per IP
- **Furniture Browsing**: 5 requests per 60 seconds per IP
- **Google OAuth**: 2 requests per 10 seconds per IP

#### Authorization Levels
- **Public**: No authentication required
- **Authenticated**: Valid JWT token required
- **Admin**: Admin role required in JWT token

### 📱 Response Formats

#### Success Responses
- **200 OK**: Request successful with data
- **201 Created**: Resource created successfully
- **204 No Content**: Request successful, no data returned

#### Pagination Response Format
```json
{
  "pageIndex": 1,
  "pageSize": 10,
  "count": 150,
  "data": [...]
}
```

#### User Response Format
```json
{
  "email": "user@example.com",
  "displayName": "John Doe",
  "token": "jwt-token-string"
}
```

### 🔧 Additional Features

#### Caching
- Furniture listings cached for 5 minutes
- Individual furniture items cached for 5 minutes
- Active offers cached for 5 minutes

#### Email Integration
- OTP verification for password reset
- Welcome emails for new users
- Order confirmation emails
- Notification emails (based on user preferences)

#### File Upload
- Furniture images support
- Multiple file formats accepted
- Automatic image optimization

## Documentation

API documentation is available through Swagger UI when running the application in development mode. Access it at `/swagger`.


## License

This project is licensed under the MIT License - see the LICENSE file for details.
- Copyright 2025 © - MIT License
- [Mohammed Mostafa](https://github.com/mo7ammedd)
- [Hasnaa Abdelrahman](https://github.com/HAsNaaAbdelRahman)
- [Albassel Abobakr](https://github.com/Bassel-11)

