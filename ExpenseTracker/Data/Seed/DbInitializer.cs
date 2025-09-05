using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        await context.Database.MigrateAsync();

        var userId = "24a2cf2c-6edd-4544-8fde-59d00718d64c";
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return;

        if (context.Categories.Any() || context.Transactions.Any()) return;

        var incomeCategories = new List<Category>
        {
            new Category { Name = "Salary", TransactionType = TransactionType.Income },
            new Category { Name = "Gift", TransactionType = TransactionType.Income },
            new Category { Name = "Bonus", TransactionType = TransactionType.Income },
            new Category { Name = "Freelance", TransactionType = TransactionType.Income },
            new Category { Name = "Interest", TransactionType = TransactionType.Income },
            new Category { Name = "Dividends", TransactionType = TransactionType.Income },
            new Category { Name = "Selling Items", TransactionType = TransactionType.Income },
            new Category { Name = "Refunds", TransactionType = TransactionType.Income },
            new Category { Name = "Investments", TransactionType = TransactionType.Income },
            new Category { Name = "Other Income", TransactionType = TransactionType.Income }
        };

        var expenseCategories = new List<Category>
        {
            new Category { Name = "Food", TransactionType = TransactionType.Expense },
            new Category { Name = "Transport", TransactionType = TransactionType.Expense },
            new Category { Name = "Rent", TransactionType = TransactionType.Expense },
            new Category { Name = "Utilities", TransactionType = TransactionType.Expense },
            new Category { Name = "Entertainment", TransactionType = TransactionType.Expense },
            new Category { Name = "Healthcare", TransactionType = TransactionType.Expense },
            new Category { Name = "Education", TransactionType = TransactionType.Expense },
            new Category { Name = "Shopping", TransactionType = TransactionType.Expense },
            new Category { Name = "Travel", TransactionType = TransactionType.Expense },
            new Category { Name = "Other Expense", TransactionType = TransactionType.Expense }
        };

        var categories = incomeCategories.Concat(expenseCategories).ToList();
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var random = new Random();

        var transactions = new List<Transaction>();

        foreach (var category in expenseCategories)
        {
            for (int i = 0; i < 1 + random.Next(1, 3); i++) 
            {
                var amount = category.Name switch
                {
                    "Food" => random.Next(5, 50),
                    "Transport" => random.Next(10, 100),
                    "Rent" => random.Next(200, 600),
                    "Utilities" => random.Next(50, 150),
                    "Entertainment" => random.Next(10, 100),
                    "Healthcare" => random.Next(20, 200),
                    "Education" => random.Next(50, 500),
                    "Shopping" => random.Next(20, 200),
                    "Travel" => random.Next(50, 1000),
                    _ => random.Next(5, 200)
                };

                transactions.Add(new Transaction
                {
                    UserId = user.Id,
                    CategoryId = category.Id,
                    TransactionType = TransactionType.Expense,
                    Amount = amount,
                    Date = DateTime.UtcNow.AddDays(-random.Next(0, 90)),
                    Description = $"Spent on {category.Name}"
                });
            }
        }

        foreach (var category in incomeCategories)
        {
            for (int i = 0; i < 1 + random.Next(1, 3); i++) 
            {
                var amount = category.Name switch
                {
                    "Salary" => random.Next(1000, 3000),
                    "Gift" => random.Next(50, 300),
                    "Bonus" => random.Next(100, 1000),
                    "Freelance" => random.Next(100, 1500),
                    "Interest" => random.Next(10, 100),
                    "Dividends" => random.Next(20, 200),
                    "Selling Items" => random.Next(5, 500),
                    "Refunds" => random.Next(5, 200),
                    "Investments" => random.Next(50, 1000),
                    _ => random.Next(10, 500)
                };

                transactions.Add(new Transaction
                {
                    UserId = user.Id,
                    CategoryId = category.Id,
                    TransactionType = TransactionType.Income,
                    Amount = amount,
                    Date = DateTime.UtcNow.AddDays(-random.Next(0, 90)),
                    Description = $"Received from {category.Name}"
                });
            }
        }

        context.Transactions.AddRange(transactions);
        await context.SaveChangesAsync();
    }
}
