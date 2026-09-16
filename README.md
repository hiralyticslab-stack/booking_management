# 🎬 Movie Booking Management System

A web-based movie booking application built with **ASP.NET MVC 5**, **.NET Framework 4.7.2**, **ADO.NET**, **SQL Server**, **jQuery**, and **AJAX**.

The application allows users to log in, select a movie category, choose a movie, calculate the booking amount, and manage booking records through standard CRUD operations.

## 📌 Features

- **User login and session tracking** using `Session["uid"]`.
- **Movie category selection** with dynamically loaded movies.
- **Cascading dropdowns** implemented with jQuery and AJAX.
- **Automatic ticket amount calculation** using:

  ```text
  Amount = Ticket Rate × Number of Tickets
  ```

- **Backend validation** before a booking is saved.
- **Booking listing and category filtering**.
- **Create, read, update, and delete operations** for bookings.
- **SQL Server or SQL Server LocalDB support** through an ADO.NET connection string.
- **Razor views** for the login, booking list, create, and edit screens.

> This project is intended for learning and local development. Before using it in production, review authentication, authorization, password storage, validation, logging, and database security.

---

## 🛠️ Technology Stack

| Area | Technology |
|---|---|
| Web framework | ASP.NET MVC 5 |
| Runtime | .NET Framework 4.7.2 or later |
| Data access | ADO.NET |
| Database | SQL Server / SQL Server LocalDB |
| Database classes | `SqlConnection`, `SqlCommand`, `SqlDataReader` |
| View engine | Razor |
| Frontend | HTML5, CSS3, Bootstrap |
| Client-side scripting | JavaScript, jQuery, AJAX |
| JSON support | Newtonsoft.Json where required by the application |

---

## 🗄️ Database Schema

The application uses four main tables. The column names below are the names referenced by the application’s SQL statements and data-access code. Confirm the suggested data types and constraints against the database used by the application.

### `Tbl_User`

Stores user login information and the user ID used during a booking.

| Column name | Suggested SQL Server type | Description |
|---|---|---|
| `User_ID` | `int` | Primary key and user identifier stored in `Session["uid"]`. |
| `User_Name` | `nvarchar(100)` | User login name or display name. |
| `User_Password` | `nvarchar(255)` | Password value; store a strong password hash in production. |

> If the login query uses additional user columns, such as `User_Email`, document those columns here and keep their names synchronized with the query.

### `Tbl_Movie_Category`

Stores movie categories such as Action, Drama, Comedy, or Animation.

| Column name | Suggested SQL Server type | Description |
|---|---|---|
| `Cat_ID` | `int` | Primary key; referenced by `Tbl_Movie.Cat_ID` and `Tbl_Booking.Cat_ID`. |
| `Cat_Name` | `nvarchar(100)` | Category name displayed in the category dropdown. |

### `Tbl_Movie`

Stores movie information, its category relationship, and ticket rate.

| Column name | Suggested SQL Server type | Description |
|---|---|---|
| `Movie_ID` | `int` | Primary key; referenced by `Tbl_Booking.Movie_ID`. |
| `Movie_Name` | `nvarchar(200)` | Movie title. |
| `Cat_ID` | `int` | Foreign key to `Tbl_Movie_Category.Cat_ID`. |
| `Rate` | `decimal(10,2)` | Ticket rate used to calculate the booking amount. |

### `Tbl_Booking`

Stores booking information and the values used to calculate the total amount.

| Column name | Suggested SQL Server type | Description |
|---|---|---|
| `Booking_ID` | `int` | Primary key; an identity column is recommended. |
| `User_ID` | `int` | Foreign key to `Tbl_User.User_ID`. |
| `Cat_ID` | `int` | Foreign key to `Tbl_Movie_Category.Cat_ID`. |
| `Movie_ID` | `int` | Foreign key to `Tbl_Movie.Movie_ID`. |
| `Tickets` | `int` | Number of tickets; should be greater than zero. |
| `Rate` | `decimal(10,2)` | Ticket rate captured for the booking, if this column exists in the database. |
| `Amount` | `decimal(12,2)` | Total booking amount: `Rate × Tickets`. |

The booking `INSERT` example used by this application explicitly references `User_ID`, `Cat_ID`, `Movie_ID`, `Tickets`, and `Amount`. Add `Rate` to the booking table and its SQL statements only if the database and data-access code store the rate with each booking.

Recommended relationships:

```text
Tbl_Movie_Category.Cat_ID  1 ──── * Tbl_Movie.Cat_ID
Tbl_User.User_ID           1 ──── * Tbl_Booking.User_ID
Tbl_Movie_Category.Cat_ID  1 ──── * Tbl_Booking.Cat_ID
Tbl_Movie.Movie_ID         1 ──── * Tbl_Booking.Movie_ID
```

---

## 🔄 How ADO.NET Works in This Application

This application uses **ADO.NET directly** instead of Entity Framework. The MVC layers work together as follows:

```text
Browser
   ↓
Razor view / jQuery AJAX request
   ↓
MVC controller
   ↓
Model or database helper class
   ↓
SqlConnection + SqlCommand
   ↓
SQL Server / LocalDB
   ↓
DataReader or affected-row result
   ↓
Controller response
   ↓
View or JSON response
```

### 1. Reading the connection string

The application reads the connection string named `constr` from `Web.config`:

```csharp
string connectionString = ConfigurationManager
    .ConnectionStrings["constr"]
    .ConnectionString;
```

Keeping the connection string in configuration prevents database details from being hard-coded throughout the application.

### 2. Opening a database connection

When a database operation is required, the data-access layer creates a `SqlConnection`:

```csharp
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    // Execute the database operation here.
}
```

The `using` statement ensures that the connection is disposed even if an exception occurs.

### 3. Executing commands with parameters

The application uses `SqlCommand` to execute SQL statements. Values should be supplied through parameters rather than string concatenation:

```csharp
const string sql = @"
    SELECT Movie_ID, Movie_Name, Rate
    FROM Tbl_Movie
    WHERE Cat_ID = @Cat_ID";

using (SqlCommand command = new SqlCommand(sql, connection))
{
    command.Parameters.AddWithValue("@Cat_ID", categoryId);

    using (SqlDataReader reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            // Map each database row to a movie model.
        }
    }
}
```

Parameterized commands help prevent SQL injection and correctly handle values sent to SQL Server.

### 4. Reading rows with `SqlDataReader`

For a `SELECT` operation, `SqlDataReader` reads the result one row at a time. The data-access class maps each field to a model object, places the objects in a list, and returns that list to the controller.

This is used for operations such as loading all bookings, loading movie categories, loading movies for a selected category, loading a booking for editing, and checking login details.

### 5. Insert, update, and delete operations

For `INSERT`, `UPDATE`, and `DELETE` statements, the application uses `ExecuteNonQuery()`:

```csharp
const string sql = @"
    INSERT INTO Tbl_Booking
        (User_ID, Cat_ID, Movie_ID, Tickets, Amount)
    VALUES
        (@User_ID, @Cat_ID, @Movie_ID, @Tickets, @Amount)";

using (SqlCommand command = new SqlCommand(sql, connection))
{
    command.Parameters.AddWithValue("@User_ID", userId);
    command.Parameters.AddWithValue("@Cat_ID", categoryId);
    command.Parameters.AddWithValue("@Movie_ID", movieId);
    command.Parameters.AddWithValue("@Tickets", tickets);
    command.Parameters.AddWithValue("@Amount", amount);

    int affectedRows = command.ExecuteNonQuery();
}
```

The returned number of affected rows can be used to determine whether the operation succeeded.

### 6. How the cascading dropdown works

The booking form follows this sequence:

1. The view loads movie categories.
2. The user selects a category.
3. jQuery sends the selected category ID to an MVC AJAX action.
4. The controller calls the ADO.NET data-access method.
5. A parameterized `SELECT` query retrieves movies for that category.
6. The controller returns the movies as JSON.
7. JavaScript fills the movie dropdown.
8. The selected movie rate is used to calculate the booking amount.

### 7. How a booking is saved

When the booking form is submitted:

1. The controller receives the posted model.
2. The logged-in user ID is obtained from `Session["uid"]`.
3. The controller validates the category, movie, ticket count, and amount.
4. The data-access class opens a `SqlConnection`.
5. A parameterized `INSERT` command is executed.
6. The user is redirected to the booking list after a successful insert.

The amount should be recalculated or verified on the server. Client-side JavaScript improves the user experience but must not be treated as the only validation layer.

---

## 📁 Project Structure

```text
booking_management/
├── App_Start/                  # Routing and MVC application configuration
├── Controllers/                # Account and booking request handling
├── Models/                     # View models and ADO.NET data-access code
├── Views/                      # Razor views for login and booking pages
├── Scripts/                    # jQuery and application JavaScript
├── Content/                    # CSS, Bootstrap, and visual assets
├── App_Data/                   # Local database files, when used
├── Web.config                  # Application settings and connection string
├── packages/                   # NuGet packages, if included in the repository
└── .gitignore                  # Visual Studio and build-output exclusions
```

Some folders may differ slightly depending on the Visual Studio project configuration.

---

## 🚀 Getting Started

### Prerequisites

- Visual Studio 2019 or later
- ASP.NET MVC 5 development workload
- .NET Framework 4.7.2 or later
- SQL Server LocalDB, SQL Server Express, or SQL Server
- A database containing the required tables and sample data

### Clone the repository

```bash
git clone https://github.com/hiralyticslab-stack/booking_management.git
cd booking_management
```

### Open the project

1. Open the solution or project file in Visual Studio.
2. Restore the NuGet packages if Visual Studio does not restore them automatically.
3. Confirm that the target framework is installed.
4. Build the solution.

### Configure the database

Open the root `Web.config` file and update the connection string named `constr` for your environment.

For SQL Server LocalDB:

```xml
<connectionStrings>
  <add name="constr"
       connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\bookingMgmtDb.mdf;Integrated Security=True;Connect Timeout=30"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

For an existing SQL Server database:

```xml
<connectionStrings>
  <add name="constr"
       connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=bookingMgmtDb;Integrated Security=True;TrustServerCertificate=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

`TrustServerCertificate=True` may be useful for local development, but production environments should use a properly configured SQL Server certificate and appropriate encryption settings.

### Prepare the database

1. Create or attach the `bookingMgmtDb` database.
2. Create the tables listed in the [Database Schema](#-database-schema) section.
3. Add at least one user account.
4. Add movie categories and movies.
5. Confirm that the foreign-key values used by bookings match existing users, categories, and movies.

This repository README does not assume a particular seed script. If you add a SQL setup script to the repository, document its exact path and run it before starting the application.

### Run the application

1. Select **IIS Express** in Visual Studio.
2. Press `Ctrl+F5` or click **Start**.
3. Open the URL displayed by Visual Studio.
4. Log in with a user account from `Tbl_User`.
5. Create and manage movie bookings.

---

## ✅ Validation and Security Notes

- Validate all posted values on the server, including ticket count, movie ID, category ID, and amount.
- Do not rely only on JavaScript validation.
- Use parameterized SQL commands for every user-supplied value.
- Do not commit real passwords, connection secrets, or production credentials.
- Store passwords using a strong password-hashing approach rather than plain text.
- Add authorization checks to every booking action so users cannot access or modify another user's records.
- Use anti-forgery tokens on state-changing MVC forms.
- Keep `debug="true"` enabled only during local development.
- Avoid exposing detailed SQL exception messages to end users.
- Use transactions when multiple related database operations must succeed or fail together.

---

## 🧰 Common Troubleshooting

### SQL connection error

- Confirm that SQL Server or LocalDB is installed and running.
- Check the `constr` connection string in `Web.config`.
- Confirm that the database name and file path are correct.
- Avoid hard-coded paths such as `D:\booking_mgmt` when running on another computer.

### Login does not work

- Confirm that `Tbl_User` contains a valid user record.
- Check that the login field names match the controller query.
- Confirm that the session value `Session["uid"]` is assigned after successful login.

### Movie dropdown is empty

- Confirm that the selected category ID exists.
- Check the browser Network tab for the AJAX response.
- Confirm that movies in `Tbl_Movie` use the correct `Cat_ID`.
- Verify that the AJAX URL matches the MVC controller action.

### Amount is incorrect

- Confirm that the movie rate is loaded correctly.
- Check that the ticket count is numeric and greater than zero.
- Recalculate the amount on the server before saving the booking.

---

## 📄 License

No license has been specified for this repository. Add a `LICENSE` file and update this section if you intend to distribute the project under a specific open-source license.

## 🤝 Contributing

1. Create a feature branch.
2. Make a focused change.
3. Test the application locally.
4. Update the README when behavior or setup requirements change.
5. Open a pull request with a clear description of the change.
