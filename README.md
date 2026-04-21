# BookSell — Online Bookstore

> A full-featured ASP.NET Core MVC e-commerce web application for online book retail.

## Features

### Customer Area
- Browse books by category with search and filter
- Book detail pages with related book suggestions
- Shopping cart (session-based, persistent across visits)
- Checkout with delivery form and payment method selection
- Order history and tracking
- User registration and login

### Admin Panel
- Dashboard with order statistics and revenue overview
- Book management (CRUD with stock tracking)
- Category, Author, and Publisher management
- Order management with status updates (Pending → Processing → Shipped → Delivered / Cancelled)

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Database | SQL Server with Entity Framework Core 9 |
| Auth | ASP.NET Core Identity |
| Frontend | Bootstrap 5, Bootstrap Icons, Google Fonts |
| ORM | Entity Framework Core 9 |
| Session | ASP.NET Core Distributed Memory Cache |

## Project Structure

```
BookSell/
├── BookSell.slnx
├── BookSell.Models/          # Domain entities + enums
│   └── Entities/
│       ├── ApplicationUser.cs
│       ├── Author.cs
│       ├── Book.cs
│       ├── Cart.cs / CartItem.cs
│       ├── Category.cs
│       ├── Enums.cs
│       ├── Order.cs / OrderItem.cs
│       └── Publisher.cs
├── BookSell.Data/           # DbContext + migrations
│   └── Data/
│       └── ApplicationDbContext.cs
└── BookSell.Web/            # MVC app
    ├── Areas/
    │   ├── Admin/           # Admin panel (controllers + views)
    │   └── Identity/        # Auth pages (login, register, logout)
    ├── Controllers/         # Customer controllers
    ├── Services/            # CartService (session-based)
    ├── ViewModels/          # DTOs/ViewModels
    └── Views/               # Shared layouts + views
```

## Getting Started

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB or full instance)

### 1. Clone & restore

```bash
git clone <your-repo-url>
cd BookSell
dotnet restore BookSell.slnx
```

### 2. Configure connection string

Open `BookSell.Web/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookSellDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Install EF tools & create database

```bash
dotnet tool install --global dotnet-ef --version 9.0.0

cd BookSell.Web
dotnet ef migrations add Init --project ../BookSell.Data --startup-project .
dotnet ef database update --project ../BookSell.Data --startup-project .
```

### 4. Run

```bash
dotnet run --project BookSell.Web --urls "http://localhost:5000"
```

Open **http://localhost:5000** in your browser.

### 5. Create admin account

Register a user at `/Identity/Account/Register`, then promote them to admin via SQL:

```sql
-- Create admin role
INSERT INTO AspNetRoles (Id, Name, NormalizedName)
VALUES ('admin-role', 'Admin', 'ADMIN');

-- Assign admin role to user
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (
    (SELECT Id FROM AspNetUsers WHERE Email = 'your@email.com'),
    'admin-role'
);
```

## Routes Overview

| Area | Route | Description |
|---|---|---|
| Customer | `/` | Homepage |
| Customer | `/Books` | Browse books |
| Customer | `/Books/Details/{id}` | Book detail |
| Customer | `/Cart` | Shopping cart |
| Customer | `/Checkout` | Checkout (requires login) |
| Customer | `/Orders` | Order history (requires login) |
| Identity | `/Identity/Account/Login` | Login |
| Identity | `/Identity/Account/Register` | Register |
| Admin | `/Admin` | Dashboard |
| Admin | `/Admin/Books` | Manage books |
| Admin | `/Admin/Categories` | Manage categories |
| Admin | `/Admin/Authors` | Manage authors |
| Admin | `/Admin/Publishers` | Manage publishers |
| Admin | `/Admin/Orders` | Manage orders |

## Database Schema

```
ApplicationUser (Identity)
├── Order (1→N)
│   └── OrderItem (N→1 Book)
├── Cart (1→N)
│   └── CartItem (N→1 Book)

Book
├── Category (N→1)
├── Author (N→1)
└── Publisher (N→1)
```

## License

MIT
