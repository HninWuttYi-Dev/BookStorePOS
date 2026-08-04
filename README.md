# Book Store POS System (MSSQL)
A web-based Point of Sale (POS) system for a bookstore that enables customers to browse and buy books easily while providing staff with tools to track inventory, process orders, and view sales metrics.

## 🛠️ Tech Stack & Database Architecture
**Database Engine:** Microsoft SQL Server (MSSQL)
**Architecture:** Relational Database with Soft Delete support for inventory tracking.

## Entity Relationship Overview
| Table | Purpose | Key Attributes |
| :--- | :--- | :--- |
| **Users** | Stores accounts for Customers, Staff, and Admins. | UserId, Email, Role |
| **Books** | Catalogs bookstore inventory (supports soft delete). | BookId, Title, Price, StockQuantity, IsDeleted |
| **Orders** | Records individual sales transactions. | OrderId, UserId, TotalPrice, OrderDate |
| **OrderItems** | Junction table linking books to specific orders. | OrderItemId, OrderId, BookId, Quantity, UnitPrice |

## 🚀 Key Features
### 🛒 Customer / Cashier Side
* **Find Books:** Search inventory by Title, Author, or Genre.
* **View Book Details:** See description, price, and stock status.
* **Shopping Cart:** Add, update, or remove books.
* **Checkout:** Display total prices and complete sales.

### 🛡️ Admin / Staff Side
* **Management:**
  * Add new books (price, initial stock).
  * View current stock levels.
  * Edit book details or update stock.
* **Soft Delete:** Mark outdated books as soft-deleted (IsDeleted = 1) to preserve historical sales records without showing them in the active catalog.
* **Sales Dashboard:** Review order history and daily sales totals.

## 🔌 API Endpoints
The following REST API endpoints are exposed by the `BookStorePOS.WebApi` project via Swagger:

### User Controller (`/api/user`)
* `GET api/user` - List all users
* `GET api/user/{id}` - Get a specific user
* `POST api/user` - Create a new user
* `PATCH api/user/{id}` - Update a user's details
* `DELETE api/user/{id}` - Delete a user

### Book Controller (`/api/book`)
* `GET api/book` - List all books
* `GET api/book/{id}` - Get a specific book
* `POST api/book` - Add a new book to inventory
* `PATCH api/book/{id}` - Update a book's details or stock
* `DELETE api/book/{id}` - Remove a book (Soft delete)

### Order Controller (`/api/order`)
* `GET api/order` - View order history 
* `GET api/order/{id}` - View a specific order (returns the order and its items)
* `POST api/order` - **Checkout!** Send the User ID and a list of items. Calculates prices and saves securely.

## 🗄️ MSSQL Setup Script
Run the following script directly in your existing database:

```sql
-- 1. Create Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Staff', 'Customer')) DEFAULT 'Customer',
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- 2. Create Books Table (Includes soft delete column)
CREATE TABLE Books (
    BookId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Author NVARCHAR(150) NOT NULL,
    Genre NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0, -- 0 = Active, 1 = Soft Deleted
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- 3. Create Orders Table
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    OrderDate DATETIME2 DEFAULT GETDATE(),
    TotalPrice DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

-- 4. Create OrderItems Table
CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    BookId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    Subtotal AS (Quantity * UnitPrice) PERSISTED, -- Calculated persisted column
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Books FOREIGN KEY (BookId) REFERENCES Books(BookId)
);
GO

-- =========================================================
-- SAMPLE DATA INSERTION
-- =========================================================

-- Insert Sample Users
INSERT INTO Users (Name, Email, PasswordHash, Role) VALUES
(N'Hnin Wutt Yi', 'admin@bookstore.com', 'hashed_pwd_admin123', 'Admin'),
(N'John Cashier', 'john.staff@bookstore.com', 'hashed_pwd_staff123', 'Staff'),
(N'Alice Smith', 'alice@gmail.com', 'hashed_pwd_cust1', 'Customer'),
(N'Bob Jones', 'bob@gmail.com', 'hashed_pwd_cust2', 'Customer');

-- Insert Sample Books
INSERT INTO Books (Title, Author, Genre, Description, Price, StockQuantity, IsDeleted) VALUES
(N'The Great Gatsby', N'F. Scott Fitzgerald', N'Classic', N'A novel about the American dream in the 1920s.', 12.99, 15, 0),
(N'To Kill a Mockingbird', N'Harper Lee', N'Classic', N'A story of racial injustice and loss of innocence.', 14.50, 10, 0),
(N'Dune', N'Frank Herbert', N'Sci-Fi', N'A science fiction masterpiece set on Arrakis.', 18.99, 8, 0),
(N'1984', N'George Orwell', N'Dystopian', N'A dystopian social science fiction novel.', 11.25, 20, 0),
(N'The Hobbit', N'J.R.R. Tolkien', N'Fantasy', N'The adventure of Bilbo Baggins in Middle-earth.', 15.00, 0, 0),
(N'Outdated IT Manual 2005', N'Tech Writer', N'Technology', N'Old tech manual no longer sold.', 5.00, 0, 1); -- Soft Deleted

-- Insert Sample Orders
INSERT INTO Orders (UserId, OrderDate, TotalPrice) VALUES
(3, '2026-08-01 10:30:00', 40.48),
(4, '2026-08-02 14:15:00', 18.99);

-- Insert Sample Order Items
INSERT INTO OrderItems (OrderId, BookId, Quantity, UnitPrice) VALUES
(1, 1, 2, 12.99), -- Alice bought 2 Gatsby
(1, 2, 1, 14.50), -- Alice bought 1 Mockingbird
(2, 3, 1, 18.99); -- Bob bought 1 Dune
GO
```
