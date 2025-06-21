# AuraDecor

AuraDecor is a comprehensive furniture management system with a modern web API backend built on ASP.NET Core 8.0. The application allows users to browse furniture items, manage their shopping cart, and place orders, while administrators can manage inventory, users, and special offers.

## Table of Contents

1. [Project Status](#project-status)
2. [Project Architecture](#project-architecture)
3. [Technology Stack](#technology-stack)
4. [Core Features](#core-features)
5. [RabbitMQ Configuration](#rabbitmq-configuration)
6. [Installation and Setup](#installation-and-setup)
7. [Docker Deployment](#docker-deployment)
8. [Health Monitoring](#health-checks)
9. [API Documentation](#api-documentation)
   - [Authentication Endpoints](#authentication-endpoints)
   - [User Profile Management](#user-profile-management)
   - [Furniture Management](#furniture-management)
   - [Offers and Discounts](#offers-and-discounts)
   - [Shopping Cart Management](#shopping-cart-management)
   - [Order Management](#order-management)
   - [Payment Processing](#payment-processing)
   - [Rating System](#rating-system)
   - [Notification System](#notification-system)
   - [Brand Management](#brand-management)
   - [Category Management](#category-management)
   - [Color Management](#color-management)
   - [Style Management](#style-management)
   - [Admin Management](#admin-management)
10. [Error Handling](#error-handling)
11. [Security and Rate Limiting](#security-and-rate-limiting)
12. [Response Formats](#response-formats)
13. [Additional Features](#additional-features)
15. [License](#license)

## Project Status

### Completed Features

- User Authentication & Authorization (JWT + Google OAuth + Twitter OAuth + Refresh Tokens)
- Furniture Catalog Management with AI-powered search
- Shopping Cart System
- Order Management with Payment Integration
- Special Offers & Discounts
- Notification System with Preferences
- Payment Processing (Stripe Integration)
- Rating & Review System
- Rate Limiting (Multiple Algorithms)
- Email Services (OTP, Notifications via RabbitMQ)
- Admin Management Dashboard
- Health Monitoring & Checks
- Brand, Category, Color, and Style Management

### Pending Development

- Coupon Module
- Recommendation Module
- Unit Testing
- Advanced Analytics

## Project Architecture

The solution follows the Clean Architecture pattern with separate layers for clear separation of concerns:

- **AuraDecor.APIs**: API controllers and presentation layer
- **AuraDecor.Core**: Domain entities, interfaces, and business rules
- **AuraDecor.Repository**: Data access and Entity Framework Core implementation
- **AuraDecor.Services**: Business logic implementation

### Architecture Diagrams

The following diagrams provide a comprehensive overview of the AuraDecor system architecture, design patterns, and data flow.

####  System Architecture Overview
![Clean Architecture Overview](./docs/clean-architecture-overview.svg)

*Clean Architecture implementation showing the separation of concerns across presentation, application, domain, and infrastructure layers.*

#### Request Processing Flow
![Request Processing Flow](./docs/request-processing-flow.svg)

*Complete request lifecycle from client to database, including middleware pipeline, authentication, caching, and error handling.*

#### Business Domains Structure
![Business Domains Overview](./docs/business-domains-overview.svg)

*Core business domains and their relationships: User Management, Product Catalog, Shopping & Orders, and Communication systems.*



#### 🔧 Technical Patterns
![Repository Pattern](./docs/repository-pattern.svg)

*Implementation of Repository and Unit of Work patterns for clean data access architecture.*

## Technology Stack

- **ASP.NET Core 8.0**: Backend framework
- **Entity Framework Core**: ORM for database operations
- **ASP.NET Core Identity**: Authentication and authorization
- **JWT Authentication**: Token-based authentication with refresh tokens
- **Google Authentication**: External authentication provider
- **Twitter Authentication**: External authentication provider
- **Stripe**: Payment processing and webhooks
- **Redis**: Distributed caching and rate limiting
- **RabbitMQ**: Message broker for asynchronous email processing
- **Swagger/Scalar**: API documentation
- **AutoMapper**: Object mapping
- **MailKit**: Email sending functionality
- **Health Checks UI**: Application health monitoring
- **Rate Limiting**: Multiple algorithms (Fixed Window, Sliding Window, Token Bucket)
- **SQL Server**: Primary database
- **Docker**: Containerization support
- **AI-Powered Search**: Text and image-based furniture search capabilities

## Core Features

- User authentication and authorization with role-based access control
- Comprehensive furniture catalog with search and filtering
- AI-powered image and text-based furniture search
- Shopping cart functionality
- Payment processing with Stripe integration
- Special offers and discounts management
- User profile and address management
- Rating and review system
- Advanced notification system with user preferences
- Admin panel for comprehensive system management
- Health monitoring and performance metrics
- Multi-algorithm rate limiting protection


### RabbitMQ Configuration

```json
{
  "RabbitMqSettings": {
    "Uri": "amqp://guest:guest@localhost:5672/",
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Port": 5672,
    "UseTls": false,
    "Queues": {
      "EmailNotifications": "email-notifications",
      "OtpEmails": "otp-emails",
      "NotificationEmails": "notification-emails"
    },
    "Exchanges": {
      "EmailExchange": "email-exchange"
    },
    "RetryPolicy": {
      "MaxRetries": 3,
      "RetryDelayMs": 5000
    }
  }
}
```

### Queue Structure

- **OTP Emails**: Handles time-sensitive one-time password emails
- **Notification Emails**: Processes system notifications (order updates, promotions, etc.)
- **Email Notifications**: General purpose email delivery
- Configure the connection string in `appsettings.json` under RabbitMqSettings section

## Installation and Setup

### Prerequisites

- .NET 8.0 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or later / Visual Studio Code

### Installation Steps

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

### Docker Deployment



#### Prerequisites for Docker Deployment

- Docker Engine 20.10+
- Docker Compose 2.0+

#### Dockerized Deployment Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/Aura-Decor/Back-end.git
   cd AuraDecor
   ```

2. Build and start the containers:
   ```bash
   docker-compose up -d
   ```

3. Access the application:
   - API: http://localhost:8080
   - Scalar Documentation: http://localhost:8080/scalar
   - RabbitMQ Management UI: http://localhost:15672 (guest/guest)

#### Docker Services

The Docker setup includes the following services:

| Service | Description | Port |
|---------|-------------|------|
| aura-decor-api | The main application | 8080 (HTTP), 8081 (HTTPS) |
| sqlserver | SQL Server 2019 | 1433 |
| redis | Redis cache server | 6379 |
| rabbitmq | RabbitMQ message broker | 5672, 15672 (Management UI) |

#### Persistent Data

All data is persisted through Docker volumes:

- `sqlserver-data`: Database files
- `redis-data`: Redis data
- `rabbitmq-data`: RabbitMQ data and configuration
- `uploads-data`: Uploaded furniture images

#### Environment Configuration

The Docker environment variables are set in the `docker-compose.yml` file.

#### Scaling the Application

For production environments.

```bash
docker-compose up -d --scale aura-decor-api=3
```
 start three instances of the application behind a load balancer.

## API Documentation

The API provides comprehensive endpoints organized by functionality. All endpoints return JSON responses and follow RESTful conventions.

### Authentication Endpoints

#### User Authentication

**POST** `/api/account/login` - User login with email and password

Request Body:
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```
- **Response**: Returns JWT token and user details
- **Rate Limited**: 2 requests per 10 seconds

**POST** `/api/account/register` - Register a new user account

Request Body:
```json
{
  "displayName": "John Doe",
  "email": "user@example.com",
  "userName": "johndoe",
  "phoneNumber": "+1234567890",
  "password": "Password123!"
}
```
- **Response**: Returns JWT token and user details
- **Rate Limited**: 2 requests per 10 seconds

#### External Authentication

**GET** `/api/account/google-login` - Initiate Google OAuth authentication
- **Rate Limited**: 2 requests per 10 seconds

**GET** `/api/account/google-response` - Handle Google OAuth callback
- **Response**: Returns JWT token and user details

**GET** `/api/account/twitter-login` - Initiate Twitter OAuth authentication

**GET** `/api/account/twitter-response` - Handle Twitter OAuth callback
- **Response**: Returns JWT token and user details

#### Token Management

**POST** `/api/account/refresh` - Refresh JWT token using refresh token

Request Body:
```json
{
  "accessToken": "expired-jwt-token",
  "refreshToken": "valid-refresh-token"
}
```
- **Response**: Returns new JWT token and refresh token

**POST** `/api/account/revoke` - Revoke refresh token

Request Body:
```json
{
  "refreshToken": "refresh-token-to-revoke"
}
```
- **Authentication**: Required (JWT Bearer token)
- **Response**: Confirmation of token revocation

#### Password Management

**POST** `/api/account/forgot-password` - Request password reset

Request Body:
```json
{
  "email": "user@example.com"
}
```
- **Response**: Sends OTP to email

**POST** `/api/account/verify-otp` - Verify OTP for password reset

Request Body:
```json
{
  "email": "user@example.com",
  "otp": "123456"
}
```
- **Response**: Returns password reset token

**POST** `/api/account/reset-password` - Complete password reset

Request Body:
```json
{
  "email": "user@example.com",
  "token": "reset-token",
  "newPassword": "NewPassword123!"
}
```

**PUT** `/api/account/updatepassword` - Update current user's password

Request Body:
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!"
}
```
- **Authentication**: Required (JWT Bearer token)

#### Utility Endpoints

**GET** `/api/account/emailexists?email={email}` - Check if email is already registered
- **Response**: Boolean value

### User Profile Management

**GET** `/api/account` - Get current authenticated user information
- **Authentication**: Required
- **Response**: User details with refreshed JWT token

**PUT** `/api/account/update` - Update user profile information

Request Body:
```json
{
  "displayName": "Updated Name",
  "phoneNumber": "+1234567890"
}
```
- **Authentication**: Required
- **Response**: Updated user details with new JWT token

#### Address Management

**GET** `/api/account/address` - Get user's saved address
- **Authentication**: Required
- **Response**: Address details or 404 if no address saved

**PUT** `/api/account/address` - Add or update user's address

Request Body:
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
- **Authentication**: Required

### Furniture Management

#### Public Endpoints

**GET** `/api/furniture` - Get all furniture with advanced filtering and pagination

Query Parameters:
- `brandId` (Guid) - Filter by specific brand
- `categoryId` (Guid) - Filter by category
- `sort` (string) - Sort options: "name", "priceAsc", "priceDesc", "newest"
- `pageIndex` (int, default: 1) - Page number for pagination
- `pageSize` (int, default: 6) - Number of items per page
- `search` (string) - Search in furniture names and descriptions
- `minPrice` (decimal) - Minimum price filter
- `maxPrice` (decimal) - Maximum price filter

- **Response**: Paginated list with total count
- **Caching**: 5 minutes
- **Rate Limited**: 5 requests per 60 seconds

**GET** `/api/furniture/{id}` - Get specific furniture item by ID
- **Response**: Detailed furniture information including images and specifications
- **Caching**: 5 minutes

#### Admin Only Endpoints

**POST** `/api/furniture` - Add new furniture item
- **Content-Type**: multipart/form-data (supports file upload for images)

Request Body:
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
- **Authentication**: Admin role required

**PUT** `/api/furniture/{id}` - Update existing furniture item
- **Authentication**: Admin role required

**DELETE** `/api/furniture/{id}` - Delete furniture item
- **Authentication**: Admin role required
- **Response**: 204 No Content on success

#### AI-Powered Search

**POST** `/api/furniture/search/text` - Search furniture using text description

Request (multipart/form-data):
- `description` (string): Natural language description of desired furniture
- `limit` (int, optional): Maximum number of results (default: 10)

Response:
```json
{
  "success": true,
  "results": [
    {
      "id": "furniture-guid",
      "name": "Modern Leather Sofa",
      "score": 0.95,
      "description": "Contemporary 3-seater leather sofa"
    }
  ]
}
```

**POST** `/api/furniture/search/image` - Search furniture using image

Request (multipart/form-data):
- `file` (file): Image file to search with
- `limit` (int, optional): Maximum number of results (default: 10)
- `color` (string, optional): Preferred color filter

Response:
```json
{
  "success": true,
  "results": [
    {
      "id": "furniture-guid",
      "name": "Similar Dining Table",
      "score": 0.88,
      "description": "Wooden dining table with similar design"
    }
  ]
}
```

### Offers and Discounts

#### Admin Endpoints

**POST** `/api/furniture/{id}/offers` - Apply special offer to furniture item

Request Body:
```json
{
  "discountPercentage": 20.0,
  "startDate": "2025-05-26T00:00:00Z",
  "endDate": "2025-06-26T23:59:59Z"
}
```
- **Authentication**: Admin role required

**DELETE** `/api/furniture/{id}/offers` - Remove offer from furniture item
- **Authentication**: Admin role required

**POST** `/api/furniture/offers/update-status` - Update status of all offers (expire old ones)
- **Authentication**: Admin role required

#### Public Endpoints

**GET** `/api/furniture/offers/active` - Get all furniture items with active offers
- **Response**: List of furniture with current discounts
- **Caching**: 5 minutes

### Shopping Cart Management

All cart endpoints require authentication.

**GET** `/api/cart` - Get user's current shopping cart
- **Response**: Cart with items, quantities, and total price

**POST** `/api/cart/add` - Add item to shopping cart

Request Body:
```json
{
  "furnitureId": "furniture-guid",
  "quantity": 2
}
```

**DELETE** `/api/cart/remove` - Remove item from cart

Request Body:
```json
{
  "furnitureId": "furniture-guid"
}
```

### Order Management

All order endpoints require authentication.

**POST** `/api/order/CreatOrder` - Create new order from cart

Query Parameters:
- `UserId` (string) - User ID
- `CartId` (Guid) - Cart ID to convert to order

- **Response**: Created order details

**GET** `/api/order/{Id}` - Get order details by user ID
- **Response**: Order information with items and status

**POST** `/api/order/cancel` - Cancel an existing order

Request Body:
```json
"order-guid-id"
```

- **Response**: Boolean indicating success with confirmation message

**GET** `/api/order/user-orders` - Get all orders for authenticated user
- **Authentication**: Required
- **Response**: List of user's orders with details and status

#### Admin Order Management

**PUT** `/api/order/update-status` - Update order status (Admin only)

Request Body:
```json
{
  "orderId": "order-guid",
  "status": "Processing"
}
```

- **Authentication**: Admin role required
- **Response**: Updated order details
- **Valid Status Values**: Pending, Processing, Shipped, Delivered, Cancelled

### Payment Processing

The payment system integrates with Stripe for secure payment processing, supporting webhooks for real-time payment status updates.

**POST** `/api/order/payment` - Create payment intent for order

Query Parameters:
- `UserId` (string) - User ID
- `CartId` (Guid) - Cart ID for payment

- **Response**: Payment intent details with client secret

**GET** `/api/payment/verify/{paymentIntentId}` - Verify payment status
- **Response**: Current payment status (Succeeded, Failed, Pending)

**GET** `/api/payment/order-details/{paymentIntentId}` - Get order details by payment intent
- **Response**: Order information with payment status

#### Webhook Processing

**POST** `/api/payment/webhook` - Stripe webhook endpoint
- **Authentication**: Webhook signature verification
- **Purpose**: Handles payment lifecycle events (success, failure, refunds, disputes)
- **Events Supported**:
  - `payment_intent.succeeded`
  - `payment_intent.payment_failed`
  - `payment_intent.canceled`
  - `charge.refunded`
  - `charge.dispute.created`
  - `charge.dispute.resolved`

#### Admin Payment Management

**POST** `/api/payment/refund` - Process refund for order (Admin only)

Request Body:
```json
{
  "orderId": "order-guid",
  "amount": 99.99,
  "reason": "Customer request"
}
```

- **Authentication**: Admin role required
- **Response**: Refund details including refund ID and status
- **Note**: If amount is omitted, processes full refund and cancels order

### Rating Management

All rating endpoints require authentication for adding a rating.

**POST** `/api/Ratings` - Add or update a rating for a product.
- **Authentication**: Required (JWT Bearer token)

Request Body:
```json
{
  "productId": "furniture-guid",
  "stars": 5,
  "review": "This is a great product!"
}
```
- **Response**: The created or updated rating object.

Response Body:
```json
{
  "id": "rating-guid",
  "stars": 5,
  "review": "This is a great product!",
  "userDisplayName": "John Doe",
  "createdAt": "2023-10-27T10:00:00Z"
}
```

**GET** `/api/Ratings/{productId}` - Get all ratings for a specific product.
- **Authentication**: Not required.

- **Response**: A list of ratings for the product.

Response Body:
```json
[
  {
    "id": "rating-guid",
    "stars": 5,
    "review": "This is a great product!",
    "userDisplayName": "John Doe",
    "createdAt": "2023-10-27T10:00:00Z"
  },
  {
    "id": "another-rating-guid",
    "stars": 4,
    "review": "Good, but could be better.",
    "userDisplayName": "Jane Smith",
    "createdAt": "2023-10-28T12:00:00Z"
  }
]
```

### Notification System

#### User Notification Endpoints (Authentication Required)

**GET** `/api/notification` - Get paginated user notifications

Query Parameters:
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Items per page

- **Response**: Paginated list of notifications

**GET** `/api/notification/unread` - Get all unread notifications
- **Response**: List of unread notifications only

**GET** `/api/notification/summary` - Get notification summary with count

Response:
```json
{
  "unreadCount": 5,
  "recentNotifications": [...]
}
```

**PUT** `/api/notification/{id}/mark-read` - Mark specific notification as read
- **Response**: 200 OK or 404 if notification not found

**PUT** `/api/notification/mark-all-read` - Mark all notifications as read
- **Response**: 200 OK or 400 if no unread notifications

**DELETE** `/api/notification/{id}` - Delete specific notification
- **Response**: 200 OK or 404 if notification not found

**DELETE** `/api/notification/all` - Delete all user notifications
- **Response**: 200 OK or 400 if no notifications to delete

#### Notification Preferences (Authentication Required)

**GET** `/api/notification/preferences` - Get user notification preferences
- **Response**: Current preference settings

**PUT** `/api/notification/preferences` - Update notification preferences

Request Body:
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

**POST** `/api/notification/admin/create` - Create notification for specific user

Request Body:
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

**POST** `/api/notification/admin/bulk` - Send bulk notifications

Request Body:
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

### Admin Management

All admin endpoints require Admin role authentication.

**GET** `/api/admin/users` - Get list of all system users
- **Response**: Array of user objects with basic information

**POST** `/api/admin/create-role` - Create new system role

Request Body:
```json
"Manager"
```
- **Request Body**: Role name as string

**POST** `/api/admin/assign-role` - Assign role to user

Request Body:
```json
{
  "email": "user@example.com",
  "roleName": "Manager"
}
```

### Brand Management

Brand management endpoints provide functionality for managing furniture brands.

#### Admin Endpoints (Admin Role Required)

**GET** `/api/brands` - Get all furniture brands
- **Response**: List of all brands with ID and name

**POST** `/api/brands` - Create new furniture brand

Request Body:
```json
{
  "name": "IKEA"
}
```
- **Response**: Created brand object with ID and name

### Category Management

Category management endpoints provide functionality for managing furniture categories.

#### Admin Endpoints (Admin Role Required)

**GET** `/api/categories` - Get all furniture categories
- **Response**: List of all categories with ID and name

**POST** `/api/categories` - Create new furniture category

Request Body:
```json
{
  "name": "Living Room"
}
```
- **Response**: Created category object with ID and name

### Color Management

Color management endpoints provide functionality for managing furniture colors.

#### Admin Endpoints (Admin Role Required)

**GET** `/api/colors` - Get all furniture colors
- **Response**: List of all colors with ID and name

**POST** `/api/colors` - Create new furniture color

Request Body:
```json
{
  "name": "Navy Blue"
}
```
- **Response**: Created color object with ID and name

### Style Management

Style management endpoints provide functionality for managing furniture styles.

#### Admin Endpoints (Admin Role Required)

**GET** `/api/styles` - Get all furniture style types
- **Response**: List of all style types with ID and name

**POST** `/api/styles` - Create new furniture style type

Request Body:
```json
{
  "name": "Contemporary"
}
```
- **Response**: Created style type object with ID and name

### Error Handling

**GET** `/errors/{code}` - Error page endpoint for HTTP status codes
- **Response**: Standardized error response format

#### Standard Error Response Format

```json
{
  "statusCode": 404,
  "message": "Lost? just like your mind"
}
```

#### Exception Response Format

```json
{
  "statusCode": 500,
  "message": "Something broke, probably my soul",
  "details": "Detailed exception information in development environment"
}
```

#### Validation Error Response Format

```json
{
  "statusCode": 400,
  "message": "What you just said makes no sense",
  "errors": [
    "Email address is in use",
    "Password must be at least 8 characters"
  ]
}
```

#### Rate Limiting Error Response

```json
{
  "statusCode": 429,
  "message": "you're banned for being gay"
}
```

#### Common Status Codes

- **400 Bad Request**: "What you just said makes no sense"
- **401 Unauthorized**: "Get the hell out of here"
- **404 Not Found**: "Lost? just like your mind"
- **429 Too Many Requests**: Rate limit exceeded
- **500 Internal Server Error**: "Something broke, probably my soul"

Rate limited endpoints include headers:
- `X-RateLimit-Limit`: Maximum number of requests allowed
- `X-RateLimit-Remaining`: Number of requests remaining
- `X-RateLimit-Reset`: Unix timestamp when the rate limit resets

## Security and Rate Limiting

### Authentication

- **JWT Bearer Tokens**: Required for protected endpoints
- **Token Format**: `Authorization: Bearer {jwt-token}`
- **Token Expiration**: Configurable (typically 24 hours)

### Rate Limiting

- **Login/Register**: 2 requests per 10 seconds per IP
- **Furniture Browsing**: 5 requests per 60 seconds per IP
- **Google OAuth**: 2 requests per 10 seconds per IP

### Authorization Levels

- **Public**: No authentication required
- **Authenticated**: Valid JWT token required
- **Admin**: Admin role required in JWT token

## Response Formats

### Success Responses

- **200 OK**: Request successful with data
- **201 Created**: Resource created successfully
- **204 No Content**: Request successful, no data returned

### Pagination Response Format

```json
{
  "pageIndex": 1,
  "pageSize": 10,
  "count": 150,
  "data": [...]
}
```

### User Response Format

```json
{
  "email": "user@example.com",
  "displayName": "John Doe",
  "token": "jwt-token-string",
  "refreshToken": "refresh-token-string",
  "refreshTokenExpiry": "2023-06-25T14:30:00Z"
}
```

## Additional Features

### Health Checks

The application includes health monitoring for critical system dependencies:

- Database health monitoring
- Redis connection monitoring
- RabbitMQ connection monitoring

Access the health status JSON endpoint at: `/health`
Access the Health Checks UI dashboard at: `/healthchecks-ui`

### Caching

- Furniture listings cached for 5 minutes
- Individual furniture items cached for 5 minutes
- Active offers cached for 5 minutes

### Email Integration

- OTP verification for password reset
- Welcome emails for new users
- Order confirmation emails
- Notification emails (based on user preferences)

### File Upload

- Furniture images support
- Multiple file formats accepted
- Automatic image optimization

## API Documentation

API documentation is available through Swagger UI when running the application in development mode. Access it at `/scalar`.

## Complete Architecture Documentation

For detailed architectural insights, the following diagrams are available in the `docs/` folder:

###  Security & Authentication
- **[JWT Authentication Flow](./docs/jwt-authentication-flow.svg)** - Complete authentication process with token management
- **[OAuth Integration Flow](./docs/oauth-integration-flow.svg)** - Google and Twitter OAuth implementation
- **[Security Middleware Pipeline](./docs/security-middleware-pipeline.svg)** - Security layers and middleware ordering

###  Domain Architecture
- **[Domain Layer Structure](./docs/domain-layer-structure.svg)** - Core domain entities and interfaces
- **[Domain Services Architecture](./docs/domain-services-architecture.svg)** - Business logic organization
- **[Specification Pattern](./docs/specification-pattern.svg)** - Query specification implementation

###  Data Management
- **[Repository Pattern](./docs/repository-pattern.svg)** - Data access pattern implementation
- **[Unit of Work Pattern](./docs/unit-of-work-pattern.svg)** - Transaction management
- **[External Dependencies](./docs/external-dependencies.svg)** - Third-party integrations

### Compliance & Legal
- **[Software Licensing](./docs/software-licensing.svg)** - Open source license overview
- **[Data Protection Framework](./docs/data-protection-framework.svg)** - Privacy and data handling
- **[User Data Handling](./docs/user-data-handling.svg)** - Data processing workflows

## License

This project is licensed under the MIT License - see the LICENSE file for details.

**Copyright 2025 © - MIT License**

### Contributors

- [Mohammed Mostafa](https://github.com/mo7ammedd)
- [Hasnaa Abdelrahman](https://github.com/HAsNaaAbdelRahman)

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/Mo7ammedd/Aura-Decor)