# ExpenseTracker

A modern web application for tracking personal expenses and income. Built with ASP.NET Core MVC and Entity Framework Core.

## Features
- User Authentication and Authorization: Secure user login and registration using ASP.NET Core Identity and Email service for verification and password recovery.
- Transaction Management: Add, edit, and delete income and expense transactions with categories.
- Category Management: Create and manage custom categories for better organization of transactions.
- Filtering and Sorting: Filter transactions by date range, category. Sort transactions by date or amount.
- Pagination: Efficiently handle large lists of transactions with pagination.
- Responsive Design: Mobile-friendly design using Bootstrap for seamless experience across devices.

## Technologies Used
- Backend: ASP.NET Core MVC, Entity Framework Core
- Frontend: HTML, CSS, JavaScript, Bootstrap
- Database: SQL Server (LocalDB for development)
- Authentication: ASP.NET Core Identity
- Email Service: SMTP for sending verification and password recovery emails

## Architecture & Design Patterns
- MVC (Model-View-Controller) Pattern: Separates application logic, UI, and data.
- Service Layer: Encapsulates business logic and data access.
- Dependency Injection: Promotes loose coupling and easier testing.
- Validation: Data annotations for model validation.
- SOLID Principles: Ensures maintainable and scalable code.

## Getting Started
### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (LocalDB for development)
- Visual Studio 2022 or later

# Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/aksza/ExpenseTracker
   cd ExpenseTracker
   ```

2. Configure the database connection string in `appsettings.json`:
   ```json
   {

	 "ConnectionStrings": {
	   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ExpenseTrackerDB;Trusted_Connection=True;MultipleActiveResultSets=true"
	 }
   }
   ```

3. Configure the email settings in `appsettings.json`:
   ```json
   {
	 "SmtpSettings": {
        "Server": "smtp.gmail.com",
        "Port": 587,
        "SenderName": "Expense Tracker",
        "SenderEmail": "your-email@gmail.com",
        "Username": "your-email@gmail.com",
        "Password": "your-app-password"
     }
   }
   ```
  

4. Install dependencies:
   ```bash
   dotnet restore
   ```

5. Apply migrations and create the database:
   ```bash
   dotnet ef database update
   ```

6. Run the application:
   ```bash
   dotnet run
   ```

7. Open your browser and navigate to `https://localhost:5001` to access the application.

## Usage
### First-Time Setup
1. Register a new user account.
2. Check your email for a verification link and verify your account.
3. Log in to the application.

### Managing Transactions
- Navigate to ExpenseTracker page: See all transactions.
- Click "Add Transaction" to add a new income or expense.
- Edit Transaction: Click on the pencil next to a transaction to modify it.
- Delete Transaction: Click on the trash can icon to remove a transaction.
- Filter Transactions: Use the filter options to view transactions by date range or category.
- Sort Transactions: Click on the column headers to sort transactions by date or amount.
- Pagination: Use the pagination controls at the bottom to navigate through pages of transactions.

### Managing Categories
- Categories are created automatically when adding a transaction with a new category name.
- Each transaction type (Income/Expense) has its own set of categories.
- Categories can be reused accross multiple transactions.

### User Profile
- Access your profile settings to update your email or change your password.
- Use the "Forgot Password" link on the login page to reset your password via email.
- Log out securely using the "Logout" button in the navigation bar.
- Delete your account from the profile settings page.
