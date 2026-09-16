# 🎬 Movie Booking Management System

A web-based Movie Booking Management Application built with **ASP.NET MVC** and **ADO.NET**. This project features user authentication, dynamic cascading dropdowns for selecting movie categories, automated ticket amount calculations, category-based filtering, and full CRUD operations.

---

## 📌 Features

- 🔐 **Authentication & Session Tracking**: Secure login routing defaulting as the startup view, with automatic user ID binding via `Session["uid"]`.
- 🎟️ **Dynamic Cascading Dropdowns**: Category selection dynamically loads associated movies using jQuery and AJAX endpoints.
- 💰 **Automated Rate & Amount Calculation**: Client-side ticket price calculation (`Amount = Rate * Tickets`) with backend validation.
- 🔍 **Category Search & Filter**: Filter booking listings dynamically by movie categories.
- ⚙️ **CRUD Operations**: Complete Create, Read, Update, and Delete operations for managing movie bookings.
- 🛡️ **SQL Exception Mitigation**: Configured connection parameters (`Encrypt=False`) for seamless local SQL Server compatibility.

---

## 🛠️ Technology Stack

* **Framework**: ASP.NET MVC 5 (.NET Framework)
* **Data Access**: ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`)
* **Database**: SQL Server
* **Frontend**: HTML5, CSS3, Bootstrap, Razor View Engine
* **Scripting**: JavaScript, jQuery, AJAX

---

## 🗄️ Database Schema

The application relies on four primary database tables:
1. `Tbl_User` – Manages user login details and user IDs.
2. `Tbl_Movie_Category` – Stores movie categories (e.g., Action, Drama, Comedy).
3. `Tbl_Movie` – Contains movie details, category mapping (`Cat_ID`), and ticket rates.
4. `Tbl_Booking` – Stores booking records including `User_ID`, `Cat_ID`, `Movie_ID`, ticket count, and total amount.

---
## Project Structure

booking_mgmt/
├── App_Start/          # RouteConfig and bundling configurations
├── Controllers/        # BookingController, AccountController
├── Models/             # Booking model and Dbhandle data access layer
├── Views/              # Razor views (Booking Index, Create, Edit, Login)
├── Scripts/            # jQuery and AJAX scripts
├── Web.config          # Connection strings and application configurations
└── .gitignore          # Git ignore rules for VS build and cache files

## Configure Connection String
1. Open Web.config in the project root.
2. Update the constr string with your local SQL Server instance details:
<connectionStrings>
  <add name="constr" 
       connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=bookingMgmtDb;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>

## 🚀 Getting Started

### Prerequisites
* Visual Studio 2019 or later
* .NET Framework 4.7.2+
* SQL Server / SQL Server Express

### Setup & Installation

1. **Clone the repository**:
   ```bash
   git clone [https://github.com/YOUR_USERNAME/booking_mgmt.git](https://github.com/YOUR_USERNAME/booking_mgmt.git)