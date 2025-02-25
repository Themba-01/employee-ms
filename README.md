Employee Management System
A full-stack application for managing employee data, built with ASP.NET Core for the backend and Blazor WebAssembly for the frontend.

Project Structure
EmployeeManagementSystem.sln: Solution file for managing multiple projects.
Server: ASP.NET Core Web API project for backend services.
Client: Blazor WebAssembly project for the user interface.
BaseLibrary: Shared components, models, and DTOs used across server and client.
ServerLibrary: Additional server-side logic, including the database context.
ClientLibrary: Additional client-side logic and services.

Technologies Used
Backend: 
ASP.NET Core 8.0
Entity Framework Core for database operations (SQL Server)
JWT for authentication
Swagger for API documentation
Frontend: 
Blazor WebAssembly
Blazored.LocalStorage for local storage
Syncfusion Blazor components for UI elements
Shared: 
C# libraries for shared models and DTOs

Backend Details
Database Context
The AppDbContext manages the following entities:

Employees, GeneralDepartments, Departments, Branches
Towns, Cities, Countries
ApplicationUsers, SystemRoles, UserRoles, RefreshTokenInfos
Vacations, VacationTypes, Overtimes, OvertimeTypes
Doctors, Sanctions, SanctionTypes

For database migrations, use:

bash
cd Server
dotnet ef database update

API Endpoints
Offers CRUD operations for employee management, HR-related functionalities like vacations, overtime, sanctions, etc.
User authentication and authorization endpoints.

Frontend Details
Application Startup (Program.cs)
Configures dependency injection for:
HttpClient with a custom handler for API calls
Authentication state management
Various services for different entities (using generic interfaces)
Syncfusion components for UI
Local storage for client-side data persistence
CustomHttpHandler: Likely used for adding headers or intercepting requests/responses.
CustomAuthenticationStateProvider: Manages authentication state for the Blazor app.

Running the Client
After starting the server, run the Client project to launch the Blazor application in your browser.

Setup Instructions
Prerequisites
.NET 8.0 SDK installed on your machine.
Visual Studio 2022 or Visual Studio Code with C# and Blazor extensions.
SQL Server for database (or configure for another database provider).

Installation
Clone the Repository:
bash
git clone git@github.com:Themba-01/employee-ms.git
or if using HTTPS:
bash
git clone https://github.com/Themba-01/employee-ms.git
Open the Solution:
Open EmployeeManagementSystem.sln with Visual Studio or load the project in Visual Studio Code.
Restore Dependencies:
Use the package manager to restore all NuGet packages.
Configure the Database:
Update the connection string in appsettings.json in the Server project to match your SQL Server instance.
Run Migrations:
In Visual Studio, use Package Manager Console:
powershell
cd Server
dotnet ef database update
Or if using command line:
bash
cd Server
dotnet ef database update

Running the Application
Server: Run the Server project to start the API.
Client: Run the Client project to start the Blazor application. Ensure the client points to the correct server URL (localhost:5162 by default).

Usage
Authentication: Users can log in, register, manage their profiles, and handle authentication tokens.
Employee Management: Add, edit, or remove employee records, manage departments, branches, etc.
HR Functions: Manage vacations, overtime, sanctions, and more through the UI.

Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
