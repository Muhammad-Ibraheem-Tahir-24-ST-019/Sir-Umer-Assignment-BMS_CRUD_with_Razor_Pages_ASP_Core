# Sir Umer Assignment — Book Management System

A book register for Sir Umer's ASP.NET Core Razor Pages assignment. It uses C# and direct ADO.NET queries for create, read, update, and delete operations.

The original SQL Server option is retained. MySQL/MariaDB support is included for running the assignment with XAMPP.

## Features

- Add books with a title, author, short description, publisher, category, and status.
- View the collection, newest entries first.
- Search by title or author.
- Edit details and mark books as **Available** or **Issued**.
- Review a book before deleting it.
- See validation errors and confirmation messages after changes.

Title and author are required. Text fields allow up to 50 characters, matching the assignment schema. Status is restricted to Available or Issued.

## Run with XAMPP

You need the **.NET 8 SDK** and **XAMPP with MySQL/MariaDB**.

1. Clone the repository:

   ```powershell
   git clone https://github.com/Muhammad-Ibraheem-Tahir-24-ST-019/Sir-Umer-Assignment-BMS_CRUD_with_Razor_Pages_ASP_Core.git
   cd Sir-Umer-Assignment-BMS_CRUD_with_Razor_Pages_ASP_Core
   ```

2. Start **MySQL** in the XAMPP Control Panel. The ASP.NET app runs separately; it does not go in `htdocs`. Apache is only needed if you want to use phpMyAdmin.

3. Import `database/setup-mysql.sql` through phpMyAdmin, or run this from PowerShell:

   ```powershell
   Get-Content database/setup-mysql.sql -Raw | & C:\xampp\mysql\bin\mysql.exe -u root -h 127.0.0.1
   ```

   This creates a separate `sir_umer_books` database. Running the script again preserves existing records. If your database account has a password, use `-p` to enter it at the prompt.

4. Configure the local connection:

   ```powershell
   dotnet user-secrets set "Database:Provider" "MySql" --project CRUD_Razor
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1;Port=3306;Database=sir_umer_books;User ID=root;Connection Timeout=5" --project CRUD_Razor
   ```

   This example matches a default local XAMPP account. If your installation uses another account, port, or password, use those settings instead. Keep credentials in user secrets, never in tracked files.

5. Build and run:

   ```powershell
   dotnet restore BMS.sln
   dotnet build BMS.sln --no-restore
   dotnet run --project CRUD_Razor
   ```

6. Open [the book register](http://localhost:5080/Books).

The included launch profile uses port 5080 and the Development environment, which loads user secrets. Leave MySQL running while using the app.

## Use SQL Server instead

SQL Server remains the default provider when `Database:Provider` is not specified.

1. Execute `database/setup.sql` in SQL Server Management Studio against your SQL Server instance.
2. Set the provider and connection:

   ```powershell
   dotnet user-secrets set "Database:Provider" "SqlServer" --project CRUD_Razor
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=BooksManagement;Trusted_Connection=True;TrustServerCertificate=True" --project CRUD_Razor
   ```

3. Run `dotnet run --project CRUD_Razor`.

Replace `(localdb)\MSSQLLocalDB` with your installed server name if needed. The certificate setting above is for local development.

## Project layout

| Location | Purpose |
| --- | --- |
| `CRUD_Razor/Models/Book.cs` | Book fields and validation rules |
| `CRUD_Razor/Pages/Books/` | CRUD pages, search, and SQL commands |
| `CRUD_Razor/Data/BookDatabase.cs` | SQL Server/MySQL connections and typed parameters |
| `CRUD_Razor/Filters/DatabaseAvailabilityFilter.cs` | Friendly handling of missing or unavailable databases |
| `CRUD_Razor/wwwroot/css/site.css` | Responsive styling |
| `database/` | Separate setup scripts for each database |
| `scripts/Test-Crud.ps1` | HTTP checks against the running app |

Each page handles its own request. Forms check validation before writing, SQL commands use typed parameters, and Razor Pages supplies anti-forgery protection. Edit and delete return 404 when a record no longer exists.

## Verification

With the app and database running, execute the checks using **PowerShell 7**:

```powershell
pwsh -File scripts/Test-Crud.ps1
```

The script creates a uniquely named temporary book, checks create/search/edit/delete, tests validation and anti-forgery handling, and removes its test record. It does not clear the collection.

The build and all 17 HTTP checks passed against XAMPP MariaDB. SQL Server support compiles but has not been integration-tested in this environment.

For a manual demonstration, add a book, search for it, change its status to Issued, and open its delete confirmation. Cancel first to confirm that viewing the page does not delete anything, then confirm the deletion.

## Troubleshooting

**Database unavailable:** Check that your database server is running, execute the matching setup script, and check the provider and connection string. Restart the app after changing settings. Book pages return HTTP 503 during database failures; the home page stays available.

**Port 5080 is already in use:** Close the previous app process or use `dotnet run --project CRUD_Razor --urls http://localhost:5082`.

This is a local classroom CRUD exercise. It does not include user accounts, borrowing history, or access permissions.

## Original assignment

Based on [MuhammadUmarHayat/BMS_CRUD_with_Razor_Pages_ASP_Core](https://github.com/MuhammadUmarHayat/BMS_CRUD_with_Razor_Pages_ASP_Core). The original commit history is retained.

This version adds the missing project/configuration files, runnable database scripts, field validation, search, confirmation messages, a revised layout, and XAMPP database support.

