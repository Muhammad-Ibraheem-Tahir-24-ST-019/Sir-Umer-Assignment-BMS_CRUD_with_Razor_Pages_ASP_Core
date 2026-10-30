# Sir Umer Assignment — Book Management System

A book register for Sir Umer's ASP.NET Core Razor Pages assignment. It uses C# and SQL Server, with direct ADO.NET queries so the create, read, update, and delete operations are easy to follow.

## What it does

- Add books with a title, author, short description, publisher, category, and status.
- View the collection, newest entries first.
- Search by title or author.
- Edit book details and mark a book as **Available** or **Issued**.
- Review a book before deleting it.
- Show validation messages and confirmation after a successful change.

Title and author are required. All text columns are limited to 50 characters to match the assignment schema. Status is restricted to Available or Issued.

## Run locally

You need the **.NET 8 SDK** and **SQL Server** (LocalDB, Express, or another SQL Server instance). Visual Studio 2022 and SQL Server Management Studio are useful but not required.

1. Clone this repository and enter its folder:

   ```powershell
   git clone https://github.com/Muhammad-Ibraheem-Tahir-24-ST-019/Sir-Umer-Assignment-BMS_CRUD_with_Razor_Pages_ASP_Core.git
   cd Sir-Umer-Assignment-BMS_CRUD_with_Razor_Pages_ASP_Core
   ```

2. Open `database/setup.sql` in SQL Server Management Studio, connect to your server, and execute it. It creates the `BooksManagement` database and `dbo.books` table. Running it again leaves existing records intact.

3. Set your connection string locally. For Windows LocalDB:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=BooksManagement;Trusted_Connection=True;TrustServerCertificate=True" --project CRUD_Razor
   ```

   For SQL Server Express, replace `(localdb)\MSSQLLocalDB` with your instance name, such as `.\SQLEXPRESS`. Use the same server where you ran the SQL script. The certificate setting above is for local development.

4. Restore, build, and start:

   ```powershell
   dotnet restore BMS.sln
   dotnet build BMS.sln --no-restore
   $env:ASPNETCORE_ENVIRONMENT = "Development"
   dotnet run --project CRUD_Razor --urls http://localhost:5080
   ```

5. Open [localhost:5080](http://localhost:5080) and choose **Open book register**.

User secrets are read in the Development environment. Alternatively, set the environment variable `ConnectionStrings__DefaultConnection`. Database credentials do not belong in tracked configuration files.

## Project layout

If a book page says **Database unavailable**, check that SQL Server is installed and running, the setup script has been executed, and `ConnectionStrings:DefaultConnection` points to that database. Restart the app after changing its environment variables. Book pages return HTTP 503 while the database is unavailable; the home page remains accessible.

| Location | Purpose |
| --- | --- |
| `CRUD_Razor/Models/Book.cs` | Book fields and validation rules |
| `CRUD_Razor/Pages/Books/` | CRUD pages, search, and SQL commands |
| `CRUD_Razor/Pages/Shared/_Layout.cshtml` | Navigation and shared page layout |
| `CRUD_Razor/wwwroot/css/site.css` | Responsive styling |
| `database/setup.sql` | Database and table setup |

Each page handles its own request. The form pages check model validation before writing to SQL Server. Queries use typed parameters, and Razor Pages supplies anti-forgery protection for POST forms. Edit and delete return 404 when a record no longer exists.

## Check the assignment

After connecting the database:

1. Add a book and confirm it appears at the top of the register.
2. Search using part of its title, then its author.
3. Edit its publisher and change its status to Issued.
4. Submit an empty title or author; the form should show an error without saving.
5. Cancel a deletion and check that the book remains.
6. Confirm the deletion and check that the record disappears.
7. Reopen the deleted book's edit URL; it should return 404.

Build and HTTP checks passed locally, including required fields, length limits, invalid status, mismatched edit IDs, and anti-forgery rejection. Database-backed CRUD still needs to be checked against a configured SQL Server instance.

The project is a local classroom CRUD exercise. It does not include user accounts, borrowing history, or permissions.

## Original assignment

Based on [MuhammadUmarHayat/BMS_CRUD_with_Razor_Pages_ASP_Core](https://github.com/MuhammadUmarHayat/BMS_CRUD_with_Razor_Pages_ASP_Core). The original commit history is retained.

This version adds the missing project/configuration files, a runnable database script, field validation, search, confirmation messages, and a revised layout.


