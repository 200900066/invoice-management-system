# invoice-management-system
Practical assessment – Invoice and Product Management System built with ASP.NET MVC, Entity Framework 6 and SQL Server.

Invoice Management System – Setup Guide

1. Clone the Repository
git clone https://github.com/200900066/invoice-management-system.git

Use the main branch (contains full code).

2. Open Project
- Open solution in Visual Studio
- Set startup project to: InvoiceManagement.Web

3. Configure Database
- Open Web.config
- Update connection string:

<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=InvoiceDB;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>

NOTE:
(localdb)\MSSQLLocalDB may not work — replace with your SQL Server instance.

4. Run Migrations (IMPORTANT)
Open Package Manager Console and run:

Update-Database

This will:
- Create the database
- Create tables
- Seed roles and users (if configured)

5. Run the Application
- Press F5 or click Run

6. Create User
Go to:
https://localhost:{PORT}/User

Create a user and assign a role (Admin / Manager / User)

7. Features
- User CRUD
- Product CRUD
- Invoice Management
- Role-based access

8. Troubleshooting
- Ensure SQL Server is running
- Check connection string
- Ensure InvoiceManagement.Web is startup project
- Rebuild solution if needed

9. Project Board
https://github.com/users/200900066/projects/3
