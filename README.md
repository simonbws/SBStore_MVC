# SBStore – E-Commerce Platform in ASP.NET Core

**SBStore** is a fully functional e-commerce web application built with **ASP.NET Core** and **Entity Framework Core**.

---

## Features

- User registration and login (roles: Customer, Company, Employee, Admin)  
- Product management (add, edit, delete, assign to categories)  
- Manage companies and product categories  
- Shopping cart and order placement process  
- Integrated payments with Stripe  
- Architecture based on Repository Pattern, Unit of Work, and Dependency Injection  
- Client-side and server-side data validation  
- Authorization and authentication with ASP.NET Core Identity
- Unit and Integration Tests

---

## Project Architecture

This project follows a layered architecture:

- Data models: Product, Category, Company, ShoppingCart, OrderHeader, OrderDetail, AppUser  
- Database context: AppDbContext, migrations, and data seeding  
- Repository and Unit of Work patterns for data access logic  
- Helper classes: SD (application constants), StripeSettings (payment configuration)  
- Presentation layer: Controllers, Views, Razor Pages, and static files in wwwroot  

---

## Data Flow Description

1. The user performs an action in the interface (for example, "Add product").  
2. The controller receives the request and uses UnitOfWork to call the correct repository.  
3. The repository communicates with AppDbContext, which manages EF Core entities.  
4. AppDbContext converts C# operations into SQL queries and saves them in the database.  
5. The result is returned to the view and displayed to the user.

---

## Validation and Security

- Validation on both client and server side  
- Authentication and roles with ASP.NET Core Identity  
- Role-based authorization  

---

## Project Structure

- Models – Data models: Product, Category, Company, ShoppingCart, OrderHeader, OrderDetail, AppUser  
- DataAccess – AppDbContext, DbSet<T> configuration, and data seeding  
- Repository – Generic Repository<T> and specific repositories (e.g., CompanyRepository)  
- UnitOfWork – UnitOfWork class implementing IUnitOfWork, coordinating repositories  
- Utility – Helper classes: SD (constants), StripeSettings (payment keys)  
- Controllers & Views – Application logic, Razor views, user and admin actions  

---

## Payments and Statuses

- Integration with Stripe API (configured in StripeSettings)  
- Order statuses: Pending, Approved, Processing, Shipped, Cancelled, Refunded  
- Payment statuses: Pending, Approved, DelayedPayment, Rejected  

---

## Technologies Used

- .NET 8.0 / ASP.NET Core MVC  
- Entity Framework Core (Code-First)  
- SQL Server (or other EF provider)  
- ASP.NET Core Identity – authentication and authorization  
- Repository Pattern and Unit of Work  
- Stripe API – payment integration  

---

## How the Application Works

1. The user performs an action in the interface (for example, "Add product").  
2. The controller receives the request and calls the appropriate repository using UnitOfWork.  
3. The repository communicates with AppDbContext.  
4. AppDbContext saves or retrieves data from the database.  
5. The controller returns the result to the view.  

---

## Installation and Running  
- To run the project locally, follow these steps:
- Clone the repository
- Update the database connection in the appsettings.json file:"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SBStore;Trusted_Connection=True;"
- Run the database migration command in the console: update-database
- Start the application with the command: dotnet run
## Author
simonbws
---
Based on the course .NET Core - The Complete Guide by Bhrugen Patel
[View the Certificate of Completion] https://www.udemy.com/certificate/UC-4594b97a-700e-4cbd-8448-306256855bd8/
---
Additional learning resources that supported this project:
---
- "Solid, Design Patterns in c# (.NET)" - [View the Certificate of Completion] https://www.udemy.com/certificate/UC-83481351-69fc-477b-a85c-43dc07d91dda/
- "ASP.NET Core Identity - Authentication & Authorization [MVC]" - [View the Certificate of Completion] https://www.udemy.com/certificate/UC-50f0c949-b453-46cd-81d4-61108a33b65a/
<img width="1056" height="786" alt="image" src="https://github.com/user-attachments/assets/30e64176-48dc-4b54-84a2-006859d697cd" />
<img width="1039" height="701" alt="image" src="https://github.com/user-attachments/assets/95001da1-a0dd-4523-a706-5a97b9303756" />
<img width="1055" height="703" alt="image" src="https://github.com/user-attachments/assets/fc63cca8-689d-4d10-aa08-9a748b409dc5" />
<img width="1060" height="700" alt="image" src="https://github.com/user-attachments/assets/16950106-6213-415e-bfaf-1d0dae650e0f" />
<img width="1057" height="698" alt="image" src="https://github.com/user-attachments/assets/2eb63b71-daad-4fa4-9a6e-c7bc8ac367c9" />
<img width="1058" height="703" alt="image" src="https://github.com/user-attachments/assets/6bb312da-4d09-43f9-9334-f91c2723b88d" />
<img width="1048" height="702" alt="image" src="https://github.com/user-attachments/assets/4ae5ec6b-fc57-4ccd-9b6b-11e64d397e17" />
<img width="1047" height="699" alt="image" src="https://github.com/user-attachments/assets/a166ee6c-03fe-4847-80f9-b0c8db9f471b" />
<img width="1057" height="700" alt="image" src="https://github.com/user-attachments/assets/b4df0ef8-f83e-4b58-ac21-d10d40fec944" />
<img width="1057" height="785" alt="image" src="https://github.com/user-attachments/assets/dfcdb116-bc05-47f4-8115-fcd8f4bfd096" />
<img width="1074" height="743" alt="image" src="https://github.com/user-attachments/assets/6ae07355-a574-4d20-90e4-4c456c33f063" />
