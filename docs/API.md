# Invoice Management System API Documentation

## Authentication

All endpoints require authentication unless marked as AllowAnonymous.

---

## Account

### Login (GET)

GET /Account/Login
Description: Returns login page

### Login (POST)

POST /Account/Login
Description: Authenticates user

Request Body:

```json
{
  "email": "user@example.com",
  "password": "your-password",
  "rememberMe": true
}
```

---

### Logout

POST /Account/LogOff
Description: Logs out current user

---

## Users

### Get All Users

GET /User
Description: Returns all users

---

### Create User (GET)

GET /User/Create
Description: Returns create user form

---

### Create User (POST)

POST /User/Create
Description: Creates a new user

Request Body:

```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "1234567890",
  "selectedRole": "User"
}
```

---

### Edit User (GET)

GET /User/Edit/{id}
Description: Returns edit form

---

### Edit User (POST)

POST /User/Edit
Description: Updates user

Request Body:

```json
{
  "id": "user-id",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "1234567890",
  "selectedRole": "User"
}
```

---

### User Details

GET /User/Details/{id}
Description: Returns user details

---

### Delete User (GET)

GET /User/Delete/{id}
Description: Returns delete confirmation page

---

### Delete User (POST)

POST /User/Delete
Description: Deletes user

Form Data:

```json
{
  "id": "user-id"
}
```

---

## Products

### Get All Products

GET /Product
Description: Returns all products

---

### Product Details

GET /Product/Details/{id}
Description: Returns product details

---

### Create Product (GET)

GET /Product/Create
Description: Returns create form

---

### Create Product (POST)

POST /Product/Create
Description: Creates product

Request Body:

```json
{
  "name": "Product Name",
  "costPerItem": 100.00,
  "quantityInStock": 10
}
```

---

### Edit Product (GET)

GET /Product/Edit/{id}
Description: Returns edit form

---

### Edit Product (POST)

POST /Product/Edit
Description: Updates product

Request Body:

```json
{
  "id": 1,
  "name": "Product Name",
  "costPerItem": 100.00,
  "quantityInStock": 10
}
```

---

### Delete Product (GET)

GET /Product/Delete/{id}
Description: Returns delete confirmation page

---

### Delete Product (POST)

POST /Product/Delete
Description: Deletes product

Form Data:

```json
{
  "id": 1
}
```

---

## Invoices

### Get Invoices

GET /Invoice
Description:

* Manager: returns all invoices
* User: returns only their invoices

---

### Create Invoice

GET /Invoice/Create
Description: Creates new invoice and redirects to details

---

### My Invoice Details

GET /Invoice/MyInvoiceDetails/{id}
Description: Returns invoice for current user

---

### Invoice Details

GET /Invoice/Details/{id}
Description: Returns invoice with products

---

### Finalize Invoice

POST /Invoice/Finalize
Description: Finalizes invoice

Request Body:

```json
{
  "invoiceId": "guid",
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 100.00
    }
  ]
}
```

---

## Reports

### Reports Dashboard

GET /Reports
Description: Returns reports dashboard

---

### Low Stock Report

GET /Reports/LowStock
Description: Returns products with stock less than or equal to 5

---

## Errors

### Access Denied

GET /Error/AccessDenied

---

## Notes

* MVC pattern (Controllers + Views)
* All POST requests require Anti-Forgery Token
* Role-based authorization is enforced
* Delete operations use POST with form data (id)
* Users and products are permanently deleted
* Invoice access is restricted by role and ownership

---

