# AuraDecor Payment Flow Documentation

## Overview
This document outlines the payment flow process in AuraDecor using Stripe for payment processing.

## Payment Flow

### 1. Create Order with Cart
- Endpoint: `POST /api/Order`
- User selects items in their cart and provides shipping information
- The system creates an order with `PaymentStatus.Pending`

### 2. Get Payment Intent
- Endpoint: `GET /api/Order/payment-intent/{cartId}`
- Frontend requests a payment intent for the specified cart
- The system calculates the total amount and returns a Stripe payment intent client secret
- This client secret is used by the frontend to initiate the payment process

### 3. Process Payment (Frontend)
- Frontend uses Stripe Elements or Stripe.js to collect card information
- Frontend confirms the payment with the client secret
- Stripe processes the payment and returns success/failure

### 4. Verify Payment Status
- Endpoint: `GET /api/Order/verify-payment/{paymentIntentId}`
- After payment processing, verify the status with Stripe
- System updates the order status based on payment result

### 5. Get Order Details
- Endpoint: `GET /api/Order/payment-order-details/{paymentIntentId}`
- Retrieve the completed order with payment status information

### 6. Webhook Processing
- Endpoint: `POST /api/Order/webhook`
- Stripe sends events to this endpoint
- System handles various payment lifecycle events (success, failure, refund, etc.)

## Testing the Payment Flow

### Setup Stripe CLI for Local Testing
1. Download and install the [Stripe CLI](https://stripe.com/docs/stripe-cli)
2. Login to your Stripe account: `stripe login`
3. Forward webhook events to your local server:
   ```
   stripe listen --forward-to https://localhost:7169/api/Order/webhook
   ```
4. Note the webhook signing secret output by the CLI and update in `appsettings.json`

### Test Successful Payment
1. Create an order through the API
2. Get payment intent for the order's cart
3. Use Stripe test cards to simulate payment:
   - Success: `4242 4242 4242 4242`
   - Any future date, any 3 digits for CVC, any postal code
4. Verify the payment status has updated to "Succeeded"
5. Check that order status has updated to "Processing"

### Test Failed Payment
1. Create an order through the API
2. Get payment intent for the order's cart
3. Use Stripe test cards to simulate payment failure:
   - Declined payment: `4000 0000 0000 0002`
4. Verify the payment status has updated to "Failed"

### Test Webhook Events
You can trigger webhook events manually using the Stripe CLI:
```
stripe trigger payment_intent.succeeded
stripe trigger payment_intent.payment_failed
stripe trigger charge.refunded
```

## Refund Process (Admin Only)
- Endpoint: `POST /api/Order/refund`
- Admin can initiate a refund for a specific order
- Partial refunds are supported by specifying an amount
- The order status updates to "Cancelled" for full refunds
