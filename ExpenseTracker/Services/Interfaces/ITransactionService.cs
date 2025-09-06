using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
using ExpenseTracker.Models.ViewModels;

public interface ITransactionService
{
    Task<TransactionsViewModel> GetUserTransactionsAsync(
        string userId,
        TransactionType type,
        int? categoryId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "Date",
        string sortOrder = "desc");

    Task<List<Category>> GetCategoriesAsync(TransactionType type);
    Task<Category?> GetCategoryByIdAsync(int categoryId);
    Task<Transaction?> GetTransactionByIdAsync(int transactionId, string userId);
    Task<Category> AddCategoryAsync(string name, TransactionType type);
    Task AddTransactionAsync(Transaction transaction);
    Task UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(int transactionId, string userId);
}