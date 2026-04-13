# 🏨 Simple Hotel System

A web-based hotel management system built with **ASP.NET Core MVC (.NET 10)** and **MySQL**. This project was developed as a personal side project to explore modern web development with C#.

---

## 📌 Features

- **Guest Management** — Register, login, update, and view guest profiles
- **Admin Panel** — Separate admin login, registration, and dashboard
- **Room Management** — Room info model with type, status, and pricing
- **Secure Authentication** — Passwords are hashed using BCrypt before storing
- **AES Encryption** — Sensitive data encryption/decryption using AES with configurable Key and IV
- **Parameterized SQL Queries** — Protection against SQL injection throughout all database operations
- **Transaction Support** — Database transaction handling via a reusable `DataAccess` class

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Language | C# |
| Database | MySQL 8.0 |
| ORM/Data Access | Raw SQL via MySql.Data |
| Frontend | Razor Views, Bootstrap 5, jQuery |
| Security | BCrypt (hashing), AES (encryption) |

---

## 🗂️ Project Structure

```
Simple Hotel System/
├── Controllers/
│   ├── HomeController.cs       # Main routing + encryption endpoint
│   ├── AdminController.cs      # Admin actions
│   ├── GuestController.cs      # Guest actions
│   └── BookingController.cs    # Booking (in progress)
├── Models/
│   ├── GuestInfo.cs
│   ├── AdminInfo.cs
│   ├── RoomInfo.cs
│   └── Booking.cs
├── Logic/
│   ├── SaveLogic.cs            # Database read/write logic
│   ├── Crypto.cs               # AES encryption/decryption
│   └── Hash.cs                 # BCrypt password hashing
├── Classes/
│   ├── DataAccess.cs           # MySQL connection & query helper
│   ├── Utility.cs              # App config helper
│   ├── Logging.cs              # Error logging
│   └── Constant.cs             # App-wide constants
└── Views/
    └── Home/
        ├── Index.cshtml
        ├── Login.cshtml
        ├── AdminLogin.cshtml
        ├── AdminRegister.cshtml
        └── AdminTable.cshtml
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [MySQL 8.0](https://dev.mysql.com/downloads/)
- Visual Studio 2022 or later (recommended)

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/simple-hotel-system.git
cd simple-hotel-system
```

### 2. Set up the database

Import the provided SQL file into MySQL:

```bash
mysql -u root -p < database1.sql
```

This will create the `database1` schema with the following tables:
- `admintable` — Admin accounts
- `guesttable` — Guest accounts
- `info` — General info/contact form

### 3. Configure the connection string

In `appsettings.json`, update the connection string and crypto settings:

```json
{
  "Data": {
    "DefaultConnection": {
      "ConnectionString": "server=localhost;database=database1;user=root;password=YOUR_PASSWORD"
    }
  },
  "Crypto": {
    "Secret": "your-16-char-key",
    "IV": "your-16-char-iv"
  }
}
```

### 4. Run the project

```bash
dotnet run
```

Or press **F5** in Visual Studio.

---

## 🔐 Security Notes

- Passwords are **never stored in plain text** — BCrypt hashing is applied before any database insert
- AES-128 encryption is available for sensitive fields via the `Crypto` class
- All SQL queries use **parameterized inputs** to prevent SQL injection

---

## 🚧 Work in Progress

- [ ] Complete booking flow (room selection, check-in/check-out, pricing)
- [ ] Guest-facing room browsing page
- [ ] Session/authentication middleware
- [ ] Input validation on forms

---

## 👤 Author

Developed by **JuGuan0716**  

